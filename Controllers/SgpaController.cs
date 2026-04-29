// Controllers/SgpaController.cs
// ─────────────────────────────────────────────────────────────────────────────
// UPDATED — added POST /api/sgpa/from-pdf endpoint.
// Existing endpoints (/calculate and /resolve) are unchanged.
//
// LEARNING CONCEPT — Controller responsibilities:
//   A controller should ONLY:
//     1. Accept the HTTP request
//     2. Validate basic input
//     3. Call a service
//     4. Return an HTTP response
//
//   Business logic (SGPA math, credit resolution) lives in services, not here.
//   This separation makes your code testable and maintainable.
// ─────────────────────────────────────────────────────────────────────────────

using Microsoft.AspNetCore.Mvc;
using SGPA_CALCULATOR.Application.Dtos;
using SGPA_CALCULATOR.Application.Interface;
using SGPA_CALCULATOR.Application.Services;
using SGPA_CALCULATOR.DTOs;

namespace SGPA_CALCULATOR.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SgpaController : ControllerBase
    {
        private readonly ISgpaService _sgpa;
        private readonly VtuCreditResolver _resolver;
        private readonly IPdfExtractorService _extractor;

        public SgpaController(
            ISgpaService sgpa,
            VtuCreditResolver resolver,
            IPdfExtractorService extractor)
        {
            _sgpa = sgpa;
            _resolver = resolver;
            _extractor = extractor;
        }


        // ════════════════════════════════════════════════════════════════════
        // NEW ENDPOINT — Upload PDF, get SGPA back in one call
        // POST /api/sgpa/from-pdf
        //
        // How to call from Swagger UI or your React frontend:
        //   Content-Type: multipart/form-data
        //   Field:        pdf  =  <the VTU result PDF file>
        //
        // What happens inside:
        //   1. Read the file bytes from the form
        //   2. Send bytes to Flask → get extracted JSON
        //   3. Map extracted JSON → SgpaRequest
        //   4. Call SgpaService.Calculate() → SgpaResponse
        //   5. Return the response
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Upload a VTU result PDF.  Returns SGPA + subject breakdown.
        /// </summary>
        [HttpPost("from-pdf")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(10 * 1024 * 1024)]   // 10 MB max — VTU PDFs are ~200 KB
        public async Task<ActionResult<SgpaResponse>> FromPdf(IFormFile? pdf)
        {
            // ── Step 1: Validate the uploaded file ────────────────────────
            if (pdf is null || pdf.Length == 0)
                return BadRequest(new { error = "No PDF file uploaded. Use field name 'pdf'." });

            if (!pdf.ContentType.Contains("pdf", StringComparison.OrdinalIgnoreCase) &&
                !pdf.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { error = "Uploaded file must be a PDF." });

            // ── Step 2: Read bytes ─────────────────────────────────────────
            byte[] pdfBytes;
            using (var ms = new MemoryStream())
            {
                await pdf.CopyToAsync(ms);
                pdfBytes = ms.ToArray();
            }

            // ── Step 3: Send to Flask extractor ───────────────────────────
            PdfExtractResult extracted;
            try
            {
                extracted = await _extractor.ExtractAsync(pdfBytes, pdf.FileName);
            }
            catch (InvalidOperationException ex)
            {
                // Flask is down, or PDF is unrecognised format
                return StatusCode(503, new { error = ex.Message });
            }

            // ── Step 4: Map extracted rows → SgpaRequest ──────────────────
            //
            // LEARNING: This is the "mapping" pattern.
            // Flask gives us PdfExtractResult (raw extracted data).
            // SgpaService wants SgpaRequest (our domain model).
            // We translate between them here.

            if (!int.TryParse(extracted.Semester, out int semester))
                semester = 0;   // SgpaService will handle unknown semester gracefully

            var request = new SgpaRequest
            {
                StudentName = extracted.StudentName,
                Usn = extracted.Usn,
                Semester = semester,
                Subjects = extracted.Subjects.Select(row => new SubjectInput
                {
                    SubjectCode = row.SubjectCode,
                    SubjectName = row.SubjectName,
                    InternalMarks = row.InternalMarks,
                    ExternalMarks = row.ExternalMarks,
                    TotalMarks= row.Total,
                    Result = row.Result

                    // ManualCreditOverride left null — VtuCreditResolver handles it
                }).ToList(),
            };

            // ── Step 5: Calculate SGPA ────────────────────────────────────
            var sgpaResponse = _sgpa.Calculate(request);

            return Ok(sgpaResponse);
        }


        // ════════════════════════════════════════════════════════════════════
        // EXISTING ENDPOINT (unchanged)
        // POST /api/sgpa/calculate
        // For direct JSON input (e.g., when pdfplumber runs on the client side)
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Calculate SGPA from JSON input (manual or from client-side extraction).
        /// </summary>
        [HttpPost("calculate")]
        public ActionResult<SgpaResponse> Calculate([FromBody] SgpaRequest request)
        {
            if (request.Subjects == null || request.Subjects.Count == 0)
                return BadRequest(new { error = "No subjects provided." });

            return Ok(_sgpa.Calculate(request));
        }


        // ════════════════════════════════════════════════════════════════════
        // DEBUG ENDPOINT (unchanged)
        // GET /api/sgpa/resolve?code=BCS301
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Debug: resolve a subject code to see what credits it maps to.
        /// </summary>
        [HttpGet("resolve")]
        public ActionResult ResolveCode([FromQuery] string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return BadRequest(new { error = "code query param required" });

            var info = _resolver.Resolve(code);
            return Ok(new
            {
                subjectCode = info.SubjectCode,
                credits = info.Credits,
                isNonCreditForSgpa = info.IsNonCreditForSgpa,
                isResolved = info.IsResolved,
                resolutionMethod = info.ResolutionMethod,
            });
        }
    }
}