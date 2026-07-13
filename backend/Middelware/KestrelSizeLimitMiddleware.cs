

// WHY "when (ex.Message.Contains("too large"))" ?
//   BadHttpRequestException is used by Kestrel for MULTIPLE reasons:
//   - Body too large
//   - Malformed headers
//   - Bad chunked encoding
//   We only want to return 413 for "too large". Other reasons should fall through
//   to ExceptionHandleMiddleware for a generic 500 or 400.

using SGPA_CALCULATOR.Application.Dtos;
using SGPA_CALCULATOR.Middelware;

namespace SGPA_CALCULATOR.Middleware
{
    public class KestrelSizeLimitMiddleware
    {
        private readonly RequestDelegate _next;     // points to the NEXT middleware in pipeline
        private readonly ILogger<KestrelSizeLimitMiddleware> _logger;

        // ASP.NET's DI calls this constructor automatically
        // RequestDelegate = a function that takes HttpContext and returns Task
        // It IS the "next step" in the pipeline
        public KestrelSizeLimitMiddleware(
            RequestDelegate next,
            ILogger<KestrelSizeLimitMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // InvokeAsync is called by ASP.NET for EVERY HTTP request
        // HttpContext = the full context: request headers, body, response, user info
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);  // run everything below this middleware
            }
            catch (BadHttpRequestException ex)
                // "when" is a C# exception filter — evaluates BEFORE catching
                // This means: only catch if the message contains "too large"
                // Other BadHttpRequestExceptions propagate normally
                when (ex.Message.Contains("too large", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning(
                    "Request body too large on {Method} {Path} — size exceeded 2MB hard limit",
                    context.Request.Method,    // GET, POST etc.
                    context.Request.Path);     // /api/sgpa/from-pdf

                // At this point the response hasn't been written yet
                // We MUST write a response, otherwise the client hangs
                context.Response.StatusCode = 413;   // 413 = Payload Too Large (official HTTP)
                context.Response.ContentType = "application/json";

                // ApiErrorResponse is your existing DTO — use it for consistency
                // Every error your API returns looks the same shape
                var errorResponse = new ApiErrorResponse
                {
                    RequestId = "ERR-" + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant(),
                    // Guid.NewGuid() = random unique ID like "550e8400-e29b..."
                    // ToString("N") = removes dashes: "550e8400e29b..."
                    // [..8] = take first 8 chars: "550E8400"
                    // Result: "ERR-550E8400" — short, traceable, user-friendly
                    StatusCode = 413,
                    Error = "File too large. VTU result PDFs are under 1MB. Please upload the correct file.",
                    TimeStamp = DateTime.UtcNow
                };

                // WriteAsJsonAsync serializes the object to JSON and writes to response body
                // We pass CamelCase policy because that's what your frontend expects
                await context.Response.WriteAsJsonAsync(errorResponse,
                    new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                    });
            }
        }
    }
}