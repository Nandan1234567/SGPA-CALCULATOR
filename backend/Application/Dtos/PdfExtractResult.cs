// DTOs/PdfExtractResult.cs
// ─────────────────────────────────────────────────────────────────────────────
// Shape of the JSON that Flask sends back after extracting a VTU PDF.
// This must exactly match what flask_app.py returns in extract_from_bytes().
//
// LEARNING CONCEPT — DTO (Data Transfer Object):
//   A DTO is a "dumb bag of data" — no logic, just properties.
//   Its only job is to carry data from one layer (Flask) to another (ASP.NET).
//   We then MAP this DTO into our domain model (SgpaRequest) in the service layer.
// ─────────────────────────────────────────────────────────────────────────────

namespace SGPA_CALCULATOR.DTOs
{
    /// <summary>
    /// Matches the JSON response from POST http://localhost:5050/extract
    /// </summary>
    public class PdfExtractResult
    {
        public string Usn { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;

        /// <summary>One entry per subject row found in the PDF.</summary>
        public List<PdfSubjectRow> Subjects { get; set; } = new();

        // Flask puts this field when extraction partially failed
        public string? Error { get; set; }
    }

    /// <summary>
    /// One row from the PDF marks table.
    /// Note: field names are camelCase here because System.Text.Json
    ///       by default deserialises camelCase JSON → PascalCase C# properties.
    /// </summary>
    public class PdfSubjectRow
    {
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int InternalMarks { get; set; }
        public int ExternalMarks { get; set; }
        public int Total { get; set; }   // reference only — we recompute
        public string Result { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
    }
}