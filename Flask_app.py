"""
flask_app.py  —  VTU PDF Extractor Microservice
================================================
Runs as a long-lived HTTP server (not called per-request like Process.Start).
ASP.NET posts the PDF file → we extract → return JSON.

Start once with: python flask_app.py
Listens on   : http://localhost:5050
Endpoint     : POST /extract   (multipart/form-data, field name = "pdf")
Health check : GET  /health

Why Flask and not Process.Start?
  - No Python cold-start (~500 ms) on every upload
  - Proper error reporting via HTTP status codes
  - Easy to run on Windows, Linux, or Azure App Service

Install dependencies once:
    pip install flask pdfplumber
"""

import re
import io
import json
import logging

import pdfplumber
from flask import Flask, request, jsonify

# ─────────────────────────────────────────────────────────────────────────────
# App setup
# ─────────────────────────────────────────────────────────────────────────────

app = Flask(__name__)
logging.basicConfig(level=logging.INFO, format="%(asctime)s  %(levelname)s  %(message)s")
log = logging.getLogger(__name__)


# ─────────────────────────────────────────────────────────────────────────────
# Pure helper functions  (no side-effects, easy to unit-test)
# ─────────────────────────────────────────────────────────────────────────────

def clean(text: object) -> str:
    """
    Collapse all whitespace in a PDF cell into single spaces.
    pdfplumber often inserts newlines inside cells — e.g. 'DIGITAL DESIGN\nAND CO'.
    """
    if text is None:
        return ""
    return " ".join(str(text).split())


def safe_int(value: object) -> int:
    """
    Convert a mark string to int.
    Handles: '38', ' 38 ', '--', 'AB', 'A', '', None → 0
    Handles: negative-looking strings '-2' (withheld) → 0
    """
    v = str(value).strip().replace("\n", "").replace(" ", "")
    # Allow plain integers only — reject '--', 'AB', 'A', etc.
    if v.lstrip("+-").isdigit() and not v.startswith("-"):
        return int(v)
    return 0


def is_subject_code(raw: str) -> bool:
    """
    Returns True only for valid VTU 22-scheme subject codes.

    Valid patterns (verified against your actual PDF):
        BCS301, BCS302, BCSL305, BSCK307, BNSK359, BCS358A, BCS306A
        BMATS101, BPHYS102, BCHES102, BESCK104A, BKSK105, BIDTK158
        BAI402, BIS701, BCA786

    Rejects header text, page numbers, blank cells, 'SUBJECT', 'CODE', etc.

    Rules:
      1. Must start with 'B'
      2. Length between 5 and 12 characters
      3. Must contain at least one digit
      4. Must not be a known header keyword
    """
    # Clean out newlines / spaces that pdfplumber puts inside cells
    c = raw.strip().replace("\n", "").replace(" ", "").upper()

    if not c:
        return False
    if len(c) < 5 or len(c) > 12:
        return False
    if c[0] != "B":
        return False
    if not any(ch.isdigit() for ch in c):
        return False

    # Reject known header words that begin with B
    BLOCKED = {"BATCH", "BRANCH", "BANGALORE", "BELAGAVI"}
    skip_prefixes = ("SUBJECT", "CODE", "COURSE", "SL", "NO", "TITLE", "MARKS")
    if c in BLOCKED:
        return False
    if any(c.startswith(p) for p in skip_prefixes):
        return False

    return True


def extract_semester(raw_text: str) -> str:
    """
    Pulls semester number from the raw text of the PDF.

    VTU PDFs print:  'Semester : 3'  (sometimes bold, sometimes after a watermark).
    We try three different regex patterns to be robust.
    """
    patterns = [
        r"Semester\s*[:\-]\s*(\d+)",          # 'Semester : 3'
        r"SEM\s*[:\-]\s*(\d+)",                # 'SEM-3'
        r"(\d)(?:st|nd|rd|th)\s+Semester",     # '3rd Semester'
    ]
    for pat in patterns:
        m = re.search(pat, raw_text, re.IGNORECASE)
        if m:
            return m.group(1).strip()
    return ""


def detect_columns(header_row: list) -> dict:
    """
    Auto-detects which column index holds which data.

    Why do we need this?
    Different VTU result PDF versions (or different colleges printing the same
    VTU template) sometimes reorder columns.  Rather than hardcoding col[2] = internal,
    we read the header row and find each column by keyword.

    Fallback values are the standard VTU layout seen in your actual PDF.
    """
    # Normalise all header cells to uppercase, stripped
    h = [clean(str(c or "")).upper() for c in header_row]

    def find(*keywords):
        """Return the first column index whose header contains any keyword."""
        for kw in keywords:
            for i, cell in enumerate(h):
                if kw in cell:
                    return i
        return None  # caller supplies fallback

    return {
        "code":     find("SUBJECT CODE", "CODE", "COURSE CODE")     or 0,
        "name":     find("SUBJECT TITLE", "COURSE TITLE", "NAME")   or 1,
        "internal": find("CIE", "INTERNAL", "INT")                  or 2,
        "external": find("SEE", "EXTERNAL", "EXT")                  or 3,
        "total":    find("TOTAL", "TOT")                            or 4,
        "result":   find("RESULT", "RES", "STATUS")                 or 5,
        "date":     find("DATE", "MONTH", "ANNOUNCED", "UPDATED")   or 6,
    }


# ─────────────────────────────────────────────────────────────────────────────
# Core extraction logic
# ─────────────────────────────────────────────────────────────────────────────

def extract_from_bytes(pdf_bytes: bytes) -> dict:
    """
    Main extraction function.  Accepts raw PDF bytes (sent from ASP.NET).
    Returns a dict matching the shape SgpaRequest expects.

    Walk-through of what pdfplumber sees in your actual PDF:

      Table 0  (student info):
        ['University Seat Number', ': 4MK22CS001']
        ['Student Name',           ': ADITHYA S'  ]

      Table 1  (marks — header + 9 data rows):
        ['Subject\\nCode', 'Subject Name', 'Internal\\nMarks', 'External\\nMarks',
         'Total', 'Result', 'Announced\\n/ Updated\\non']
        ['BCS301', 'MATHEMATICS FOR\\nCOMPUTER SCIENCE', '38', '22', '60', 'P', '2024-05-02']
        ...
    """
    result = {
        "usn":         "",
        "studentName": "",
        "semester":    "",
        "subjects":    [],
    }

    seen_codes = set()  # de-duplicate subjects that appear on multiple pages

    # Wrap bytes in a seekable buffer — pdfplumber needs a file-like object
    with pdfplumber.open(io.BytesIO(pdf_bytes)) as pdf:

        for page_num, page in enumerate(pdf.pages):
            raw_text = page.extract_text() or ""

            # ── Semester from free text (usually on first page) ───────────
            if not result["semester"]:
                sem = extract_semester(raw_text)
                if sem:
                    result["semester"] = sem
                    log.info("Semester detected: %s (page %d)", sem, page_num + 1)

            # ── Tables ────────────────────────────────────────────────────
            tables = page.extract_tables() or []

            for table in tables:
                if not table or not table[0]:
                    continue

                # Replace None cells with empty string throughout
                table = [
                    [c if c is not None else "" for c in row]
                    for row in table
                ]

                # Identify table type by first cell of header row
                header_text = clean(str(table[0][0])).upper()

                # ─────────────────────────────────────────────────────────
                # Student info table
                # Identified by: first cell contains "UNIVERSITY SEAT" or "USN"
                # ─────────────────────────────────────────────────────────
                if "UNIVERSITY SEAT" in header_text or "USN" in header_text:
                    for row in table:
                        if len(row) < 2:
                            continue
                        label = clean(str(row[0])).upper()
                        value = clean(str(row[1])).lstrip(": ")

                        if not result["usn"] and (
                            "UNIVERSITY SEAT" in label or "USN" in label
                        ):
                            result["usn"] = value
                            log.info("USN: %s", value)

                        if not result["studentName"] and (
                            "STUDENT NAME" in label or "NAME" in label
                        ):
                            result["studentName"] = value
                            log.info("Student: %s", value)

                # ─────────────────────────────────────────────────────────
                # Marks table
                # Identified by: first cell contains "SUBJECT" or "COURSE"
                # ─────────────────────────────────────────────────────────
                elif "SUBJECT" in header_text or "COURSE" in header_text:
                    col = detect_columns(table[0])
                    log.info("Column mapping: %s", col)

                    for row in table[1:]:   # skip the header row itself
                        # Pad short rows to avoid IndexError
                        while len(row) <= max(col.values()):
                            row.append("")

                        raw_code = str(row[col["code"]])
                        code = clean(raw_code).replace("\n", "").upper()

                        if not is_subject_code(code):
                            continue        # skip header repeats, blank rows

                        if code in seen_codes:
                            continue        # skip duplicate (multi-page PDF)
                        seen_codes.add(code)

                        name     = clean(str(row[col["name"]]))
                        internal = safe_int(row[col["internal"]])
                        external = safe_int(row[col["external"]])
                        total    = safe_int(row[col["total"]])
                        res      = clean(str(row[col["result"]]))
                        date     = clean(str(row[col["date"]]))

                        # If total is missing/zero but marks are present, compute it
                        if total == 0 and (internal > 0 or external > 0):
                            total = internal + external

                        result["subjects"].append({
                            "subjectCode":   code,
                            "subjectName":   name,
                            "internalMarks": internal,
                            "externalMarks": external,
                            "total":         total,      # for reference only
                            "result":        res,
                            "date":          date,
                        })
                        log.info("  Subject: %s  INT=%d  EXT=%d", code, internal, external)

    log.info("Extraction complete — %d subjects found", len(result["subjects"]))
    return result


# ─────────────────────────────────────────────────────────────────────────────
# Flask routes
# ─────────────────────────────────────────────────────────────────────────────

@app.route("/health", methods=["GET"])
def health():
    """
    ASP.NET calls this on startup to verify Flask is alive.
    Returns 200 {"status": "ok"} when ready.
    """
    return jsonify({"status": "ok"}), 200


@app.route("/extract", methods=["POST"])
def extract():
    """
    Main endpoint — called by PdfExtractorService.cs in ASP.NET.

    Request  : multipart/form-data  →  field "pdf" = the uploaded PDF file
    Response : JSON  { usn, studentName, semester, subjects: [...] }
    Errors   : JSON  { error: "..." }  with appropriate HTTP status code

    ASP.NET code that calls this:
        var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(pdfBytes), "pdf", "result.pdf");
        var response = await _http.PostAsync("http://localhost:5050/extract", content);
    """
    # ── Validate request ──────────────────────────────────────────────────
    if "pdf" not in request.files:
        log.warning("Request arrived without a 'pdf' field")
        return jsonify({"error": "Missing 'pdf' field in multipart form data"}), 400

    file = request.files["pdf"]
    if not file.filename:
        return jsonify({"error": "Empty filename"}), 400

    # ── Read bytes ────────────────────────────────────────────────────────
    pdf_bytes = file.read()
    if not pdf_bytes:
        return jsonify({"error": "PDF file is empty"}), 400

    log.info("Received PDF — %d bytes — %s", len(pdf_bytes), file.filename)

    # ── Extract ───────────────────────────────────────────────────────────
    try:
        data = extract_from_bytes(pdf_bytes)
    except Exception as exc:
        log.exception("Extraction failed")
        return jsonify({"error": f"PDF extraction failed: {exc}"}), 500

    # ── Validate output ───────────────────────────────────────────────────
    if not data["subjects"]:
        return jsonify({
            "error": "No subject rows found. The PDF may not be a VTU result sheet, "
                     "or the table format is unrecognised.",
            "rawData": data,
        }), 422

    return jsonify(data), 200


# ─────────────────────────────────────────────────────────────────────────────
# Entry point
# ─────────────────────────────────────────────────────────────────────────────

if __name__ == "__main__":
    # debug=False in production / on the server
    # threaded=True so ASP.NET can make parallel requests
    app.run(host="0.0.0.0", port=5050, debug=False, threaded=True)