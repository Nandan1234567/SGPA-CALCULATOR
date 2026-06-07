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
using SGPA_CALCULATOR.Application.Mappers;
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
        private readonly ILogger<SgpaController> _log;
        

        public SgpaController(
            ISgpaService sgpa,
            VtuCreditResolver resolver,
            IPdfExtractorService extractor, ILogger<SgpaController> logger)
        {
            _sgpa = sgpa;
            _resolver = resolver;
            _extractor = extractor;
            _log = logger;
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
        [RequestSizeLimit(2 * 1024 * 1024)]   //  — VTU PDFs are ~200 KB
        [Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("pdf-upload")]
        public async Task<ActionResult<SgpaResponse>> FromPdf(IFormFile? pdf)
        {

            var sw = System.Diagnostics.Stopwatch.StartNew();   

            // ── Step 1: Validate the uploaded file ────────────────────────
            if (pdf is null || pdf.Length == 0)
                throw new ArgumentException(
                    "No file uploaded. Please select your VTU result PDF.");    

            // ContentType check: browsers set this automatically when user picks a file.
            // .EndsWith check: fallback for tools like Postman that might not set ContentType.
            if (!pdf.ContentType.Contains("pdf", StringComparison.OrdinalIgnoreCase) &&
                !pdf.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException(
                    "The uploaded file is not a PDF. Please upload your VTU result PDF.");

            //if (pdf.Length >  1024 * 1024)
            //    return BadRequest(new
            //    {
            //        error = "File too large. VTU result PDFs are under 1MB. Please upload your official VTU result PDF."
            //    });

            // ── Step 2: Read bytes ─────────────────────────────────────────
            byte[] pdfBytes;
            using (var ms = new MemoryStream())
            {
                await pdf.CopyToAsync(ms);
                pdfBytes = ms.ToArray();
            }

            // ── Step 3: Send to Flask extractor ───────────────────────────
            //PdfExtractResult extracted;
            // No try/catch — let exception bubble up to middleware
            var extracted = await _extractor.ExtractAsync(pdfBytes, pdf.FileName);

            // ── Step 4: Map extracted rows → SgpaRequest ──────────────────
            //
            // LEARNING: This is the "mapping" pattern.
            // Flask gives us PdfExtractResult (raw extracted data).
            // SgpaService wants SgpaRequest (our domain model).
            // We translate between them here.
            // File: Controllers/SgpaController.cs - FromPdf method

           
            if (!int.TryParse(extracted.Semester, out int semester))
                semester = 0;

            // PdfResultMapper.ToSgpaRequest() ALSO parses extracted.Semester internally:
            var request = PdfResultMapper.ToSgpaRequest(extracted);

            _log.LogInformation("... Sem={Sem} ...", semester);


            // ── Step 5: Calculate SGPA ────────────────────────────────────


            // here we are injected the di for sgpa service ,
            //      in request it has infromation sgpa service request requeired and it gives in sgpa response format
            var sgpaResponse = _sgpa.Calculate(request);

            sw.Stop();
            _log.LogInformation(
                "PDF processed — USN={Usn} Sem={Sem} Subjects={Count} " +
                "FileSize={Bytes}bytes Duration={Ms}ms",
                extracted.Usn,
                semester,
                request.Subjects.Count,
                pdf.Length,
                sw.ElapsedMilliseconds);

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
        [Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("calculate")]    // A DD

        public ActionResult<SgpaResponse> Calculate([FromBody] SgpaRequest request)
        {
                if (request.Subjects == null || request.Subjects.Count == 0)
                return BadRequest(new { error = "No subjects provided." });




            // With this fix:
            //   We force totalMarks = 45 + 30 = 75 before service sees it
            //   Service computes grade from 75 — correct


            if (request.Subjects.Count < 3)
            {
                // Log a warning — you want to know this is happening
                // Don't return error — maybe a student genuinely has 2 subjects (makeup exam)
                _log.LogWarning(
                    "Calculate called with only {Count} subjects for USN={Usn} — possible frontend bug",
                    request.Subjects.Count, request.Usn);
            }

            for (int i = 0; i < request.Subjects.Count; i++)
            {
                var sub = request.Subjects[i];

                // Guard: null/empty subject code crashes VtuCreditResolver.Resolve()
                // ArgumentException → middleware → 400
                if (string.IsNullOrWhiteSpace(sub.SubjectCode))
                    throw new ArgumentException(
                        $"Subject at index {i} has empty SubjectCode. All subjects must have a code.");

                // Clamp marks to valid VTU range — don't reject, just correct
                // WHY clamp instead of reject?
                // Rejecting means 1 bad subject blocks 8 good ones.
                // Clamping means the weird value gets corrected, calc still runs.
                // Frontend should prevent this, but backend defends anyway.
                //
                // Internal: 0-50 (VTU CIE max is 50)
                // External: 0-100 (most subjects 0-50, but 200-mark subjects exist)
                //           We use 100 as upper bound — SgpaService handles scaling
                sub.InternalMarks = Math.Clamp(sub.InternalMarks, 0, 50);
                sub.ExternalMarks = Math.Clamp(sub.ExternalMarks, 0, 100);

                // Always recompute total from clamped values
                // WHY: user might send internal=45 but forget to update totalMarks=71
                // If we trust their totalMarks, grade is computed from stale data
                // Recomputing here means SgpaService always sees correct total
                sub.TotalMarks = sub.InternalMarks + sub.ExternalMarks;
            }


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