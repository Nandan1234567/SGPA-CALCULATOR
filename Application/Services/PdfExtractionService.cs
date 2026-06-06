// Application/Services/IPdfExtractorService.cs
// Application/Services/PdfExtractorService.cs
// ─────────────────────────────────────────────────────────────────────────────
// LEARNING CONCEPT — Interface + Implementation separation:
//
//   IPdfExtractorService defines the CONTRACT  (what it can do)
//   PdfExtractorService   provides the BEHAVIOUR (how it does it)
//
//   ASP.NET's Dependency Injection wires them together in Program.cs:
//     services.AddScoped<IPdfExtractorService, PdfExtractorService>();
//
//   This means you can swap implementations (e.g., replace Flask with a
//   direct pdfplumber call) without changing anything in the controller.
// ─────────────────────────────────────────────────────────────────────────────

using System.Net.Http.Json;
using SGPA_CALCULATOR.Application.Exceptions;
using SGPA_CALCULATOR.DTOs;

namespace SGPA_CALCULATOR.Application.Services
{
    // ── Interface ────────────────────────────────────────────────────────────
    public interface IPdfExtractorService
    {
        /// <summary>
        /// Sends raw PDF bytes to the Flask microservice and returns the
        /// extracted result.  Throws HttpRequestException if Flask is down.
        /// </summary>
        Task<PdfExtractResult> ExtractAsync(byte[] pdfBytes, string fileName);
    }


    // ── Implementation ────────────────────────────────────────────────────────
    public class PdfExtractorService : IPdfExtractorService
    {
        // IHttpClientFactory creates managed HttpClient instances.
        // NEVER create a new HttpClient per request — it exhausts socket ports.
        // We register a named client "Flask" in Program.cs.
        private readonly IHttpClientFactory _httpFactory;
        private readonly ILogger<PdfExtractorService> _log;

        public PdfExtractorService(IHttpClientFactory httpFactory,
                                   ILogger<PdfExtractorService> log)
        {
            _httpFactory = httpFactory;
            _log = log;
        }

        public async Task<PdfExtractResult> ExtractAsync(byte[] pdfBytes, string fileName)
        {
            // ── Build multipart form data ─────────────────────────────────
            // This is exactly what a browser sends when you pick a file in an <input type="file">.
            // Field name "pdf" must match what Flask expects: request.files["pdf"]
            using var content = new MultipartFormDataContent();
            using var byteContent = new ByteArrayContent(pdfBytes);
            byteContent.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
            content.Add(byteContent, "pdf", fileName);

            // ── Send to Flask ─────────────────────────────────────────────
            var client = _httpFactory.CreateClient("Flask");


            // Generate a short correlation ID for THIS specific PDF extraction request.
            // This same ID gets:
            //   - Logged in C# before we call Flask
            //   - Sent to Flask as HTTP header X-Request-Id
            //   - Logged by Flask on receipt and on completion
            // Result: grep "REQ-A3F9C201" in EITHER log → see the full story
            var requestId = "REQ-" + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();

            // TryAddWithoutValidation: safer than Add() — won't throw if header somehow exists
            // X-Request-Id is the industry standard header name for correlation IDs
            // Used by AWS, Azure, Google Cloud, nginx — your frontend can also read it
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Request-Id", requestId);

            _log.LogInformation(
                "[{RequestId}] Sending PDF to Flask — {Bytes} bytes — {FileName}",
                requestId,
                pdfBytes.Length,     // how big was the file
                fileName);


            HttpResponseMessage response;
            try
            {
                response = await client.PostAsync("/extract", content);
            }
            catch (HttpRequestException ex)
            {
                // Flask is not running — give the developer a clear message

                throw new InvalidOperationException(
            "Cannot reach Flask PDF service on http://localhost:5050. " +
            "Make sure Flask is running: python flask_app.py", ex);
            }

            // File: Application/Services/PdfExtractorService.cs
            // FIXED — reads the HTTP status code and throws the RIGHT exception type

            if (!response.IsSuccessStatusCode)
            {
                // Read the error body Flask sent back
                // Flask sends: {"error": "No subject rows found..."} or {"error": "PDF extraction failed..."}
                var errorBody = await response.Content.ReadAsStringAsync();

                _log.LogError(
                    "Flask returned {StatusCode} on {FileName}: {Body}",
                    (int)response.StatusCode,
                    fileName,
                    errorBody);

                // 422 = Unprocessable Content = Flask understood the request
                //       but the PDF is not a VTU result sheet.
                //       THIS IS THE CALLER'S FAULT → PdfValidationException → 422
                if ((int)response.StatusCode == 422)
                {
                    // PdfValidationException message goes directly to the user
                    // (your middleware does: 422 => ex.Message)
                    // So write a human-friendly message here, not a technical one
                    throw new PdfValidationException(
                        "The uploaded file is not a recognised VTU result PDF. " +
                        "Please download your result directly from results.vtu.ac.in.");
                }

                // Any other non-success (500, 400 from Flask) = Flask itself broke
                // Not the caller's fault → InvalidOperationException → 503
                throw new InvalidOperationException(
                    $"PDF extraction service failed ({(int)response.StatusCode}). Please try again.");

            }


            // System.Text.Json deserialises camelCase JSON automatically
            // because we configured JsonNamingPolicy.CamelCase in Program.cs
            var result = await response.Content.ReadFromJsonAsync<PdfExtractResult>();

            if (result is null)
                throw new InvalidOperationException("Flask returned empty JSON body.");

            _log.LogInformation(
                "Extracted {Count} subjects for USN={Usn}", result.Subjects.Count, result.Usn);

            return result;
        }
    }

}