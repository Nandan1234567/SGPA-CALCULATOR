

// Polly — retry policy for Flask HTTP calls
using Microsoft.EntityFrameworkCore;
using Polly;
using SGPA_CALCULATOR.Application.Interface;
using SGPA_CALCULATOR.Application.Services;
using SGPA_CALCULATOR.Infrastructure.Data;
using SGPA_CALCULATOR.Middelware;
using SGPA_CALCULATOR.Middleware;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Disable automatic 400 response on ModelState errors.
        // Our ExceptionHandleMiddleware handles all errors uniformly.
        // Without this suppression, ASP.NET would return its own 400 format
        // that doesn't match our ApiErrorResponse shape.
        options.SuppressModelStateInvalidFilter = true;
    })
    .AddJsonOptions(opt =>
        opt.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase);  // snake → camelCase serialisation

builder.Services.AddEndpointsApiExplorer();  // required by Swashbuckle to discover endpoints
builder.Services.AddSwaggerGen(c =>
{
    // This generates the OpenAPI spec at /swagger/v1/swagger.json.
    // Swagger UI reads this file. If swagger.json returns 500, the UI shows
    // "Failed to load API definition" — which is what you were seeing.
    c.SwaggerDoc("v1", new() { Title = "VTU SGPA Calculator API", Version = "v1" });
});

// ── Database ───────────────────────────────────────────────────────────────────
// SgpaDbContext is Scoped (EF Core default) — one instance per HTTP request.
// That's correct because DbContext is NOT thread-safe.
builder.Services.AddDbContext<SgpaDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Named HttpClient for Flask microservice ────────────────────────────────────
// WHY named client not direct new HttpClient():
//   new HttpClient() per request = socket exhaustion (TIME_WAIT state sockets pile up).
//   IHttpClientFactory pools and manages the underlying HttpMessageHandler lifecycle.
//   This is the correct industry pattern for outbound HTTP calls in ASP.NET.
var flaskBaseUrl = builder.Configuration["FlaskService:BaseUrl"]
    ?? throw new InvalidOperationException(
        "FlaskService:BaseUrl is not configured. " +
        "Add it to appsettings.json: \"FlaskService\": { \"BaseUrl\": \"http://localhost:5050\" }");
// ?? throw = fail-fast at startup if config is missing.
// Missing URL = every PDF upload would throw at runtime with a confusing error.
// Failing at startup makes the problem obvious immediately.

// GetValue<int> with default 30:
// TimeoutSeconds is OPTIONAL — if missing, use 30 as safe default.
// BaseUrl is REQUIRED — no safe default exists, so we throw above.
var flaskTimeout = builder.Configuration.GetValue<int>("FlaskService:TimeoutSeconds", 30);

builder.Services.AddHttpClient("Flask", client =>
{
    client.BaseAddress = new Uri(flaskBaseUrl);  // all requests on this client go to Flask
    client.Timeout = TimeSpan.FromSeconds(10);    // 10s per attempt (not counting retries)
    // Why 10s not 30s: VTU PDFs extract in ~0.5-2 seconds.
    // 10 seconds is generous. If Flask takes 10s, something is wrong.
    // Fail fast, retry once (below). Total max wait = 10s + 300ms + 10s = ~20s.
})
.AddTransientHttpErrorPolicy(policy =>
    // Polly retry: fires on HTTP 5xx responses OR network failures (no route, refused).
    // "Transient" = temporary, expected-to-recover errors.
    // 429 (rate limit) and 4xx are NOT transient — Polly does NOT retry those.
    policy.WaitAndRetryAsync(
        retryCount: 1,                                          // try once, then retry once = 2 total attempts
        sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(300),  // wait 300ms between attempts
        onRetry: (outcome, timespan, attempt, context) =>
        {
            // Console.WriteLine because this runs inside the factory pipeline,
            // not inside a controller where ILogger is available.
            // In production, attach a delegating handler with ILogger instead.
            Console.WriteLine($"[FlaskRetry] Attempt {attempt} failed, retrying in 300ms. " +
                              $"Reason: {outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()}");
        }
    )
);

// ── Application services ───────────────────────────────────────────────────────

// VtuCreditResolver — MUST be Singleton, not Scoped.
// WHY: Its constructor loads ALL SubjectMasters rows from SQL into a
//      Dictionary<string, SubjectMaster> (_sem12Cache).
// Singleton = constructor runs ONCE at app startup → cache lives for app lifetime.
//   Every Resolve("BCS301") call = O(1) dictionary lookup, zero DB hits.
// Scoped = constructor runs on EVERY HTTP request → DB query on every upload.
//   That's the N+1 problem re-introduced at the service level.
// The constructor uses IServiceScopeFactory to create a temporary DB scope —
//   that's the pattern for Singletons that need a Scoped dependency at startup only.
builder.Services.AddSingleton<VtuCreditResolver>();

// SgpaService — Scoped is correct here.
// SgpaService.Calculate() is pure computation — no shared state between requests.
// Each request gets its own instance. Disposed after the response is sent.
builder.Services.AddScoped<ISgpaService, SgpaService>();

// PdfExtractorService — Scoped is correct here.
// Uses IHttpClientFactory (Singleton) via constructor injection.
// IHttpClientFactory is safe to inject into Scoped services.
builder.Services.AddScoped<IPdfExtractorService, PdfExtractorService>();

// ── CORS — allow React dev server ─────────────────────────────────────────────
// CORS = Cross-Origin Resource Sharing.
// Browser security blocks requests from origin A (localhost:5173) to origin B (localhost:5100).
// This policy tells ASP.NET to add the correct CORS headers so the browser allows it.
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>()
    ?? throw new InvalidOperationException(
        "Cors:AllowedOrigins is not configured. " +
        "Add to appsettings.json: \"Cors\": { \"AllowedOrigins\": [\"http://localhost:5173\"] }");

builder.Services.AddCors(opt =>
    opt.AddPolicy("ReactApp", p =>
        p.WithOrigins(allowedOrigins)   // only allow configured origins — not *
         .AllowAnyHeader()              // allow Content-Type, Authorization, X-Request-Id etc
         .AllowAnyMethod()));           // allow GET, POST, OPTIONS (OPTIONS = preflight)

// ── NO RATE LIMITER ────────────────────────────────────────────────────────────
// Rate limiting belongs to nginx in production.
// nginx rate-limits at the TCP/IP layer — BEFORE the request reaches .NET.
// Application-level rate limiting fires AFTER .NET parses the multipart body.
// You've already paid the parsing cost. nginx prevents that cost entirely.
// Docker + nginx reverse proxy will handle this via nginx.conf limit_req_zone.

// ── File upload size limits ────────────────────────────────────────────────────
// Two layers of enforcement:
// 1. FormOptions: limits multipart/form-data body during model binding
// 2. Kestrel: hard OS-level TCP read limit
// Both are set to 2MB. VTU PDFs are ~150-400KB so this is generous.

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(opt =>
{
    // MaxRequestBodySize for multipart/form-data binding.
    // This fires during IFormFile binding in the controller action.
    opt.MultipartBodyLengthLimit = 2 * 1024 * 1024;  // 2 MB = 2,097,152 bytes
});

builder.WebHost.ConfigureKestrel(options =>
{
    // Hard limit at the TCP read level — before ASP.NET even sees the body.
    // Kestrel throws BadHttpRequestException("Request body too large")
    // which KestrelSizeLimitMiddleware catches and returns 413.
    options.Limits.MaxRequestBodySize = 2 * 1024 * 1024;  // 2 MB

    // MinRequestBodyDataRate: if client uploads slower than 100 bytes/second
    // for more than 10 seconds, Kestrel kills the connection.
    // Prevents slow-upload DoS attacks (slowloris-style against upload endpoint).
    options.Limits.MinRequestBodyDataRate = new Microsoft.AspNetCore.Server.Kestrel.Core.MinDataRate(
        bytesPerSecond: 100,
        gracePeriod: TimeSpan.FromSeconds(10));
});

// Response compression: Gzip responses if client sends Accept-Encoding: gzip.
// Reduces payload size for SgpaResponse JSON (can be ~3-5KB for 10 subjects).
// EnableForHttps = false: don't compress HTTPS responses (BREACH attack risk).
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = false;
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
});

var app = builder.Build();

// ── Middleware pipeline (ORDER MATTERS) ───────────────────────────────────────
// Each UseMiddleware call adds a "layer" to the pipeline.
// Requests pass through layers top-to-bottom.
// Responses pass through layers bottom-to-top (reverse order).

// Layer 1: KestrelSizeLimitMiddleware
// Catches BadHttpRequestException("too large") → returns 413 JSON.
// Must be FIRST so oversized requests are rejected before any parsing.
app.UseMiddleware<KestrelSizeLimitMiddleware>();

// Layer 2: ExceptionHandleMiddleware
// Catches all unhandled exceptions from layers below → returns structured JSON.
// Must be BEFORE controllers so it catches their exceptions.
app.UseMiddleware<ExceptionHandleMiddleware>();

// NO app.UseRateLimiter() here — nginx owns this in production.

// Swagger UI — development only. Never expose in production (leaks API structure).
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();     // serves /swagger/v1/swagger.json
    app.UseSwaggerUI();   // serves /swagger UI at /swagger/index.html
}

// Compress response bodies before sending over the wire.
app.UseResponseCompression();

// Add CORS headers to responses. Must be before MapControllers.
// If CORS is after controllers, the response headers are never added.
app.UseCors("ReactApp");

// Redirect HTTP → HTTPS in production.
// NOT in development because localhost:5100 doesn't have a valid certificate.
// Trying to redirect dev traffic causes ERR_TOO_MANY_REDIRECTS.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();  // required even if no auth — removes "middleware not configured" warning
app.MapControllers();    // registers all [ApiController] routes discovered via reflection

// ── Auto-migrate on startup (dev only) ────────────────────────────────────────
// db.Database.Migrate() runs pending migrations and creates the DB if it doesn't exist.
// This is how the SubjectMasters table with 93 seed rows gets created on first run.
// In production: run "dotnet ef database update" manually or via CI/CD pipeline.
// Never auto-migrate in production — it could apply breaking schema changes during traffic.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();  // create a temporary DI scope
    var db = scope.ServiceProvider.GetRequiredService<SgpaDbContext>();  // get DbContext
    db.Database.Migrate();  // apply any pending migrations
}

app.Run();  // blocks here — starts Kestrel, listens for connections