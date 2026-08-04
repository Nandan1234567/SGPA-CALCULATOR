using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Reflection.Metadata;
using System.Text.Json;
using SGPA_CALCULATOR.Application.Exceptions;

namespace SGPA_CALCULATOR.Middleware
{
  public class ExceptionHandleMiddleware
  {

    // di on request

    private readonly RequestDelegate _next;

    private readonly ILogger _logger;



    public ExceptionHandleMiddleware(RequestDelegate next, ILogger<ExceptionHandleMiddleware> logger)
    {

      _next = next;
      _logger = logger;

    }

    public async Task InvokeAsync(HttpContext context)
    {


      try
      {


        await _next(context);
      }
      catch (Exception ex)
      {


        await HandleExceptionAsync(context, ex);

      }

    }


    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {



      if (ex is OperationCanceledException && context.RequestAborted.IsCancellationRequested)
      {
        _logger.LogInformation("Request cancelled by client on {Method} {Path}",
            context.Request.Method, context.Request.Path);
        return; // connection is already gone — no point sending a response
      }


      // ── STEP 1: Generate unique error ID ─────────────────────────────

      string errorId = "ERR-" + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();

      // ── STEP 2: Categorize the exception → HTTP status code ───────────
      //

      // The pattern switch (ex switch) checks the RUNTIME TYPE of ex.
      // Order matters — more specific types first, general types last.
      int statusCode = ex switch
      {
        // ── CALLER ERRORS (4xx) ─────────────────────────────────────
        // These are the caller's fault. Be specific so they can fix it.

        ArgumentNullException => (int)HttpStatusCode.BadRequest,      // 400
        ArgumentOutOfRangeException => (int)HttpStatusCode.BadRequest,      // 400
        ArgumentException => (int)HttpStatusCode.BadRequest,      // 400
                                                                  // Put ArgumentNullException BEFORE ArgumentException
                                                                  // because ArgumentNullException IS-A ArgumentException
                                                                  // The switch takes the FIRST match. More specific = first.


        PdfValidationException => 422,//(wrong PDF = user error)


        BadHttpRequestException => 413,

        System.Text.Json.JsonException => (int)HttpStatusCode.BadRequest,



        // ── DEPENDENCY FAILURES (5xx) ───────────────────────────────
        // External systems failed. Not your code, not the caller.

        HttpRequestException => (int)HttpStatusCode.ServiceUnavailable, // 503
                                                                        // HttpRequestException = network-level failure (Flask unreachable)
                                                                        // This comes from System.Net.Http when the TCP connection fails


        // here i think task cancel is not required i think 499 is better what do you think?
        TaskCanceledException => (int)HttpStatusCode.GatewayTimeout,     // 504
                                                                         // TaskCanceledException = the HttpClient timeout expired
                                                                         // Your Flask client has 30 second timeout — if Flask takes >30s → this

        // SqlException removed - using PostgreSQL now
        // Npgsql throws NpgsqlException but it inherits from DbException
        System.Exception e when e.GetType().Name.Contains("Npgsql") => (int)HttpStatusCode.ServiceUnavailable,// 503


        DbUpdateException => (int)HttpStatusCode.ServiceUnavailable, // 503
                                                                     // DbUpdateException = EF Core failed to save/update

        InvalidOperationException => (int)HttpStatusCode.ServiceUnavailable, // 503
                                                                             // YOUR manually thrown exceptions (Flask returned bad JSON, etc.)

        InvalidDataException => (int)HttpStatusCode.BadRequest,


        _ => (int)HttpStatusCode.InternalServerError // 500
                                                     // _ is the "catch-all" — everything not matched above → 500
      };

      // ── STEP 3: Build the message the USER sees ───────────────────────
      //   ✓ Generic messages for 500s
      //   ✓ ex.Message for 400s (you wrote these messages — they're safe)
      //   ✓ Partial ex.Message for 503s (dependency name is okay to share)
      string userMessage = statusCode switch
      {
        400 => ex.Message,
        // Your ArgumentException messages are written by YOU:
        // "No subjects provided." — safe to show

        413 => "Invalid request. Please check your file and try again.",

        422 => ex.Message + " Kindly upload downloaded  pdf from the phone",

        503 => "A required service is temporarily unavailable. Please try again.",


        504 => "The request took too long. Please try again.",
        // Friendly timeout message


        _ => "Something went wrong. Please try again."
        // ALL 500s → ALWAYS generic. No exceptions.
      };


      _logger.LogError(
          ex,                // Full exception with stack trace — first param for ILogger
          "Unhandled exception [{ErrorId}] {ExceptionType} on {Method} {Path}",
          errorId,
          ex.GetType().Name,               // "SqlException", "NullReferenceException"
          context.Request.Method,           // "POST", "GET"
          context.Request.Path              // "/api/sgpa/from-pdf"
      );

      // ── STEP 5: Build response object ─────────────────────────────────
      var errorResponse = new ApiErrorResponse
      {
        // left side defined and  right side are output
        RequestId = errorId,
        StatusCode = statusCode,
        Error = userMessage,
        TimeStamp = DateTime.UtcNow,  // Always UTC — server may be in different timezone
      };


      // IMPORTANT: Check if response has already started.
      // If the controller streamed partial data before throwing (rare but possible),
      // we can't change headers anymore. ASP.NET will handle it.
      if (context.Response.HasStarted)
      {
        // Response already partially sent — we can't change it now.
        // Just log that we couldn't send our error response.
        _logger.LogWarning(
            "Response already started for [{ErrorId}] — cannot write error JSON",
            errorId);
        return;
      }

      context.Response.ContentType = "application/json";  // Tell browser: this is JSON
      context.Response.StatusCode = statusCode;           // 400, 503, 500, etc.

      // Serialize ApiErrorResponse to JSON
      // PropertyNamingPolicy.CamelCase → "ErrorId" becomes "errorId" in JSON
      // This matches what your React frontend expects
      var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
      {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
      });

      // WriteAsync sends the JSON string as the response body
      await context.Response.WriteAsync(json);
      // After this line: the student's browser receives the error JSON
    }
  }


}

