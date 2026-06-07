// Program.cs — FIXED & CLEANED
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Polly;
using SGPA_CALCULATOR.Application.Interface;
using SGPA_CALCULATOR.Application.Services;
using SGPA_CALCULATOR.Infrastructure.Data;
using SGPA_CALCULATOR.Middelware;
using SGPA_CALCULATOR.Middleware;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ── JSON serialisation — camelCase policy ─────────────────────────────────────
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    })
    .AddJsonOptions(opt =>
        opt.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "VTU SGPA Calculator API", Version = "v1" });
});

// ── Database ───────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<SgpaDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Named HttpClient for Flask microservice ────────────────────────────────────
//
// FIX #1: Read config ONCE with a fail-fast guard, then REUSE those variables.
// Never read the same config key twice — it creates inconsistency risk.
//
var flaskBaseUrl = builder.Configuration["FlaskService:BaseUrl"]
    ?? throw new InvalidOperationException(
        "FlaskService:BaseUrl is not configured. " +
        "Add it to appsettings.json or set FLASKSERVICE__BASEURL environment variable.");

var flaskTimeout = builder.Configuration.GetValue<int>("FlaskService:TimeoutSeconds", 30);
// File: Program.cs
builder.Services.AddHttpClient("Flask", client =>
{
    client.BaseAddress = new Uri(flaskBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(10);  // ← reduce from 30 to 10
})
.AddTransientHttpErrorPolicy(policy =>
    policy.WaitAndRetryAsync(
        retryCount: 1,                           // ← reduce from 2 to 1
        sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(300),  // ← 300ms flat
        onRetry: (outcome, timespan, attempt, context) =>
        {
            Console.WriteLine($"[FlaskRetry] Attempt {attempt} failed, retrying in 300ms. " +
                              $"Reason: {outcome.Exception?.Message}");
        }
    )
);
// ── Application services ───────────────────────────────────────────────────────
builder.Services.AddSingleton<VtuCreditResolver>();
builder.Services.AddScoped<ISgpaService, SgpaService>();
builder.Services.AddScoped<IPdfExtractorService, PdfExtractorService>();

// ── CORS — allow React dev server ─────────────────────────────────────────────
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>()
    ?? throw new InvalidOperationException(
        "Cors:AllowedOrigins is not configured. " +
        "Add it to appsettings.json or set CORS__ALLOWEDORIGINS__0 environment variable.");

builder.Services.AddCors(opt =>
    opt.AddPolicy("ReactApp", p =>
        p.WithOrigins(allowedOrigins)
         .AllowAnyHeader()
         .AllowAnyMethod()));

// File: Program.cs
// REPLACE your entire AddRateLimiter block with this:

builder.Services.AddRateLimiter(options =>
{
    // ── PDF UPLOAD: 10 per IP per minute ──────────────────────────────────────
    // AddPolicy with RateLimitPartition = per-IP buckets
    // Each IP gets its own independent counter
    // Student A's 10 uploads don't affect Student B's quota

    options.AddPolicy("pdf-upload", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            // partitionKey = the "bucket identifier"
            // RemoteIpAddress = the client's IP address as seen by Kestrel
            // If behind nginx proxy: use X-Forwarded-For header instead (see note below)
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: _ => new System.Threading.RateLimiting.FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,                    // 10 uploads per IP per minute
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,                      // reject immediately, don't queue
                QueueProcessingOrder =
                    System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst,
            }
        ));

    // ── JSON CALCULATE: 30 per IP per minute ──────────────────────────────────
    options.AddPolicy("calculate", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: _ => new System.Threading.RateLimiting.FixedWindowRateLimiterOptions
            {
                PermitLimit = 30,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
            }
        ));

    // ── 429 Response ──────────────────────────────────────────────────────────
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        // Add Retry-After header: tells browser/Postman how long to wait
        context.HttpContext.Response.Headers["Retry-After"] = "60";
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            error = "Too many requests. Please wait 1 minute and try again.",
            statusCode = 429,
            retryAfter = "60 seconds"
        }, cancellationToken);
    };
});

// ── File upload size ───────────────────────────────────────────────────────────
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(opt =>
{
    opt.MultipartBodyLengthLimit = 2 * 1024 * 1024; // 2 MB
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 2 * 1024 * 1024; // 2 MB

    options.Limits.MinRequestBodyDataRate = new Microsoft.AspNetCore.Server.Kestrel.Core.MinDataRate(
        bytesPerSecond: 100,
        gracePeriod: TimeSpan.FromSeconds(10));
});

builder.Services.AddResponseCompression(options => {
    options.EnableForHttps = false;
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
});


var app = builder.Build();

// ── Middleware pipeline ────────────────────────────────────────────────────────
app.UseMiddleware<KestrelSizeLimitMiddleware>();
app.UseMiddleware<ExceptionHandleMiddleware>();


app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCompression();
app.UseCors("ReactApp");
app.UseHttpsRedirection();
app.UseAuthorization();
  
app.MapControllers();

// ── Auto-migrate on startup (dev only) ────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<SgpaDbContext>();
    db.Database.Migrate();
}

app.Run();