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

            _log.LogInformation("Sending {Bytes} bytes to Flask /extract", pdfBytes.Length);

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
                    "Did you run start.bat (Windows) or python flask_app.py?", ex);
            }

            // ── Parse response ────────────────────────────────────────────
            if (!response.IsSuccessStatusCode)
            {
                // Flask returned 400/422/500 — read the error message
                var errorBody = await response.Content.ReadAsStringAsync();
                _log.LogError("Flask returned {Code}: {Body}", response.StatusCode, errorBody);
                throw new InvalidOperationException(
                    $"Flask extraction failed ({response.StatusCode}): {errorBody}");
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