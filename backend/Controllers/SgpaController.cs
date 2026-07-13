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
        private readonly IHttpClientFactory _httpFactory;

        public SgpaController(
            ISgpaService sgpa,
            VtuCreditResolver resolver,
            IPdfExtractorService extractor,
            ILogger<SgpaController> log,
            IHttpClientFactory httpFactory)
        {
            _sgpa = sgpa;
            _resolver = resolver;
            _extractor = extractor;
            _log = log;
            _httpFactory = httpFactory;
        }

        /// <summary>
        /// Uploads a VTU result PDF and returns the calculated SGPA and per-subject breakdown.
        /// </summary>
        [HttpPost("from-pdf")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(2 * 1024 * 1024)]
        public async Task<ActionResult<SgpaResponse>> FromPdf(IFormFile pdf)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            // Validate file presence and basic PDF content type/extension
            if (pdf is null || pdf.Length == 0)
                throw new ArgumentException("No file uploaded. Please select your VTU result PDF.");

            if (!pdf.ContentType.Contains("pdf", StringComparison.OrdinalIgnoreCase) &&
                !pdf.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("The uploaded file is not a PDF. Please upload your VTU result PDF.");

            // Buffer the incoming HTTP file stream into a byte array for Flask
            byte[] pdfBytes;
            using (var ms = new MemoryStream())
            {
                await pdf.CopyToAsync(ms);
                pdfBytes = ms.ToArray();
            }

            // Send raw bytes to the Flask microservice for OCR/text extraction
            var extracted = await _extractor.ExtractAsync(pdfBytes, pdf.FileName);

            // Map raw Flask output into domain request model
            if (!int.TryParse(extracted.Semester, out int semester))
                semester = 0;

            var request = PdfResultMapper.ToSgpaRequest(extracted);
            _log.LogInformation("Mapped PDF — Sem={Sem} Subjects={Count}", semester, request.Subjects.Count);

            // Execute synchronous SGPA calculation
            var sgpaResponse = _sgpa.Calculate(request);

            sw.Stop();
            _log.LogInformation(
                "PDF processed — USN={Usn} Sem={Sem} Subjects={Count} FileSize={Bytes}bytes Duration={Ms}ms",
                extracted.Usn,
                semester,
                request.Subjects.Count,
                pdf.Length,
                sw.ElapsedMilliseconds);

            return Ok(sgpaResponse);
        }

        /// <summary>
        /// Calculates SGPA from raw JSON input (used for manual entry and frontend edits).
        /// </summary>
        [HttpPost("calculate")]
        public ActionResult<SgpaResponse> Calculate([FromBody] SgpaRequest request)
        {
            if (request.Subjects == null || request.Subjects.Count == 0)
                return BadRequest(new { error = "No subjects provided." });

            if (request.Subjects.Count < 3)
                _log.LogWarning("Calculate called with only {Count} subjects — USN={Usn}. Possible frontend bug or makeup exam.",
                    request.Subjects.Count, request.Usn);

            // Sanitize and normalize subject marks before passing to domain layer
            for (int i = 0; i < request.Subjects.Count; i++)
            {
                var sub = request.Subjects[i];

                if (string.IsNullOrWhiteSpace(sub.SubjectCode))
                    throw new ArgumentException($"Subject at index {i} has empty SubjectCode. All subjects must have a valid VTU subject code.");

                // Clamp CIE (0-50) and SEE (0-100) to valid VTU boundaries
                sub.InternalMarks = Math.Clamp(sub.InternalMarks, 0, 50);
                sub.ExternalMarks = Math.Clamp(sub.ExternalMarks, 0, 100);

                // Recompute total to prevent frontend calculation discrepancies
                sub.TotalMarks = sub.InternalMarks + sub.ExternalMarks;
            }

            return Ok(_sgpa.Calculate(request));
        }

        /// <summary>
        /// Debug tool: resolves a subject code to inspect credit assignment and resolution strategy.
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

        /// <summary>
        /// Warm-up endpoint triggered on initial load to wake ASP.NET and check Flask service health.
        /// </summary>
        [HttpGet("ping")]
        public async Task<IActionResult> Ping()
        {
            try
            {
                var client = _httpFactory.CreateClient("Flask");
                var response = await client.GetAsync("/health");

                if (!response.IsSuccessStatusCode)
                    _log.LogWarning("Flask returned {Status} during warm-up ping", (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                // Absorb exceptions so ASP.NET still reports as awake even if Flask is booting
                _log.LogWarning("Flask ping failed — service may be starting: {Message}", ex.Message);
            }

            return Ok(new { status = "awake", timestamp = DateTime.UtcNow });
        }
    }
}