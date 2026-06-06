//using Microsoft.EntityFrameworkCore;
//using Microsoft.Identity.Client;
//using System.Runtime.InteropServices;
//using static System.Runtime.InteropServices.JavaScript.JSType;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Net;
using System.Reflection.Metadata;
using System.Text.Json;
using SGPA_CALCULATOR.Application.Exceptions;

namespace SGPA_CALCULATOR.Middelware
{
    public class ExceptionHandleMiddleware
    {

        // di on request 

        private readonly RequestDelegate _next;

        private readonly ILogger _logger;



        public ExceptionHandleMiddleware(RequestDelegate next, ILogger<ExceptionHandleMiddleware>logger )
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
         
            // IMPORTANT: SqlException and DbUpdateException need special handling
            // because they're not base .NET — they come from specific libraries.
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


              


                // ── DEPENDENCY FAILURES (5xx) ───────────────────────────────
                // External systems failed. Not your code, not the caller.

                HttpRequestException => (int)HttpStatusCode.ServiceUnavailable, // 503
                                                                                // HttpRequestException = network-level failure (Flask unreachable)
                                                                                // This comes from System.Net.Http when the TCP connection fails


                // here i think task cancel is not required i think 499 is better what do you think?
                TaskCanceledException => (int)HttpStatusCode.GatewayTimeout,     // 504
                                                                                 // TaskCanceledException = the HttpClient timeout expired
                                                                                 // Your Flask client has 30 second timeout — if Flask takes >30s → this

                SqlException => (int)HttpStatusCode.ServiceUnavailable, // 503
                                                                        // SqlException = SQL Server is down, wrong password, network failure
                                                                        // This is from Microsoft.Data.SqlClient — needs using directive

                DbUpdateException => (int)HttpStatusCode.ServiceUnavailable, // 503
                                                                             // DbUpdateException = EF Core failed to save/update
                                                                             // Wraps SqlException but also covers constraint violations

                InvalidOperationException => (int)HttpStatusCode.ServiceUnavailable, // 503
                                                                                     // YOUR manually thrown exceptions (Flask returned bad JSON, etc.)
                                                                                     // See PdfExtractorService.cs — you throw InvalidOperationException

                // ── YOUR CODE BUGS (500) ────────────────────────────────────
                // NullReference, IndexOutOfRange, JsonException, etc.
                // If these reach here, you have a bug to fix.

                InvalidDataException => (int)HttpStatusCode.BadRequest,


                _ => (int)HttpStatusCode.InternalServerError // 500
                                                             // _ is the "catch-all" — everything not matched above → 500
            };

            // ── STEP 3: Build the message the USER sees ───────────────────────
            //
            // SECURITY RULE: Never expose internal details for 500 errors.
            // What you must NEVER send to the user:
            //   ✗ ex.Message for 500s (might contain SQL query, file path, class name)
            //   ✗ ex.StackTrace (shows your code structure — helps attackers)
            //   ✗ ex.GetType().Name (attackers learn what you're using)
            //   ✗ Inner exception details
            //
            // What IS safe to send:
            //   ✓ Generic messages for 500s
            //   ✓ ex.Message for 400s (you wrote these messages — they're safe)
            //   ✓ Partial ex.Message for 503s (dependency name is okay to share)
            string userMessage = statusCode switch
            {
                400 => ex.Message,
                // Your ArgumentException messages are written by YOU:
                // "No subjects provided." — safe to show

                413 => "File too large. VTU result PDFs are under 500KB.", 


            422 => ex.Message+" Kindly upload downloaded  pdf from the phone",



                503 => "A required service is temporarily unavailable. Please try again.",
                // SEMI-GENERIC for 503: don't say "Flask on port 5050 is down"
                // User doesn't need technical details. Just "try again later."
                // The real message is in the logs.

                504 => "The request took too long. Please try again.",
                // Friendly timeout message


                _ => "Something went wrong. Please try again."
                // ALL 500s → ALWAYS generic. No exceptions.
            };

            // ── STEP 4: LOG the full exception ────────────────────────────────
            //
            // This is what YOU see (not the user).
            // LogError() writes:
            //   - The errorId (so you can find this specific occurrence)
            //   - The HTTP method and path (which endpoint failed)
            //   - The FULL exception: type + message + stack trace
            //   - The inner exception (if any — e.g., DbUpdateException wraps SqlException)
            //
            // In Azure Application Insights, these become searchable log entries.
            // You search by errorId, by exception type, by path, by time range.

            // And in the logging section, change this:
            // Don't log cancellations as errors — they pollute your logs with noise




            _logger.LogError(
                ex,                // Full exception with stack trace — first param for ILogger
                "Unhandled exception [{ErrorId}] {ExceptionType} on {Method} {Path}",
                errorId,
                ex.GetType().Name,               // "SqlException", "NullReferenceException"
                context.Request.Method,           // "POST", "GET"
                context.Request.Path              // "/api/sgpa/from-pdf"
            );
            // Your log output looks like:
            // [ERROR] Unhandled exception [ERR-550E8400] SqlException on POST /api/sgpa/from-pdf
            //         Microsoft.Data.SqlClient.SqlException: A network-related instance-specific error...
            //           at VtuCreditResolver..ctor() line 42
            //           at SgpaController..ctor() line 28

            // ── STEP 5: Build response object ─────────────────────────────────
            var errorResponse = new ApiErrorResponse
            {
                // left side defined and  right side are output
               RequestId = errorId,
                StatusCode = statusCode,
                Error = userMessage,
                TimeStamp = DateTime.UtcNow,  // Always UTC — server may be in different timezone
            };
            
            // ── STEP 6: Write HTTP response ────────────────────────────────────
            // At this point, the controller never sent a response (it threw).
            // We write the response manually.
            //
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



////STEP 1 — Create folder

////  Middleware/ inside your project root





////STEP 2 — Create ApiErrorResponse.cs

////  4 properties: Error, StatusCode, RequestId, Timestamp
////  Namespace: SGPA_CALCULATOR.Middleware





////STEP 3 — Create ExceptionHandlerMiddleware.cs

////  Constructor: takes RequestDelegate +ILogger
////  InvokeAsync: try { await _next(context) } catch { HandleExceptionAsync }
////HandleExceptionAsync:
////    → generate requestId(Guid, first 8 chars)
////    → switch on exception type → statusCode + userMessage
////    → log (LogError for real errors, LogWarning for cancelled)
////    → check context.Response.HasStarted
////    → set ContentType, StatusCode
////    → build ApiErrorResponse, serialize, WriteAsync




////STEP 4 — Program.cs

////  app.UseMiddleware<ExceptionHandlerMiddleware>();
////FIRST line after var app = builder.Build();


//        // di for 2 things   request delegation and ilogger

//        private readonly RequestDelegate _next;

//        private readonly ILogger _logger;


//        // creating the constructor // why automatic? only that reasons , di reason idk proper reasons

//        // if i not create a constructor wwhy in private property shows null

//        public ExceptionHandleMiddleware(RequestDelegate requestDelegate, ILogger logger)
//        {

//            _next = requestDelegate;
//            _logger = logger;

//        }


//        // writing try catches using this property refernce

//        // logger used to log info and _next to read all middleware at one go and 
//        // i am all along  taking refernce of this

//        // i  am not able to write  this invoke  like how to use this
//        // do i need to log idk?

//        // see here this invoke inside or outside exceptionhandle middleware constructor 
//        // i dont think so 
//        public async Task invoke(HttpContext context, ILogger logger)
//        {

//            try
//            {

//                _next(context);

//            }

//            catch (Exception ex)
//            {
//                // how to catch exception


//            }



//        }


//        //public Task<HandleExceptionAsync>

//        // how to create



//        public async  void HandleException( )
//        {

//        }

//            // i am not able to write the status code and how to write errors , how to  use this log check build i think may be i was writing in memory and hint i feel 







