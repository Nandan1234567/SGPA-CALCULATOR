using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using SGPA_CALCULATOR.Application.Interface;
using SGPA_CALCULATOR.Application.Services;
using SGPA_CALCULATOR.Infrastructure.Data;
using SGPA_CALCULATOR.Middleware;
using SGPA_CALCULATOR.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ── Controllers ────────────────────────────────────────────────────────────────
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
      options.SuppressModelStateInvalidFilter = true;
    })
    .AddJsonOptions(opt =>
        opt.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase);

builder.Services.AddEndpointsApiExplorer();

// Only add Swagger in dev. Never expose in production.
if (builder.Environment.IsDevelopment())
{
  builder.Services.AddSwaggerGen(c =>
  {
    c.SwaggerDoc("v1", new() { Title = "VTU SGPA Calculator API", Version = "v1" });
  });
}

// ── Database: PostgreSQL ───────────────────────────────────────────────────────
// Connection string comes from environment variable in production:
// ConnectionStrings__DefaultConnection="Host=postgres;Database=sgpa_db;..."
builder.Services.AddDbContext<SgpaDbContext>(opt =>
    opt.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions =>
        {
          // Retry on transient PostgreSQL failures (network hiccup, connection reset)
          npgsqlOptions.EnableRetryOnFailure(
              maxRetryCount: 3,
              maxRetryDelay: TimeSpan.FromSeconds(5),
              errorCodesToAdd: null);
        }
    ));

// ── Health Checks ──────────────────────────────────────────────────────────────
builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "postgresql",
        tags: new[] { "db", "ready" }
    );

// ── Flask HTTP Client with Resilience Pipeline ─────────────────────────────────
//  need BOTH retry AND circuit breaker:


var flaskBaseUrl = builder.Configuration["FlaskService:BaseUrl"]
    ?? throw new InvalidOperationException(
        "FlaskService:BaseUrl is not configured. " +
        "Set it in appsettings.json or via environment variable FLASKSERVICE__BASEURL");

builder.Services.AddHttpClient("Flask", client =>
{
  client.BaseAddress = new Uri(flaskBaseUrl);
  // Timeout is PER ATTEMPT. Total request time = timeout × (1 + retries).
  // Gunicorn worker timeout is 60s. Set slightly above to let Gunicorn kill it first.
  client.Timeout = TimeSpan.FromSeconds(70);
})
.AddResilienceHandler("flask-pipeline", pipeline =>
{
  // Layer 1: Per-attempt timeout (outermost — controls single attempt duration)
  pipeline.AddTimeout(TimeSpan.FromSeconds(65));

  // Layer 2: Retry (wraps circuit breaker)
  pipeline.AddRetry(new HttpRetryStrategyOptions
  {
    MaxRetryAttempts = 1,
    Delay = TimeSpan.FromMilliseconds(300),
    BackoffType = DelayBackoffType.Constant,
    UseJitter = false,
    // Only retry on 5xx or network errors, NOT on 4xx (those are caller bugs)
    ShouldHandle = args => ValueTask.FromResult(
        args.Outcome.Exception is HttpRequestException ||
        ((int?)args.Outcome.Result?.StatusCode) >= 500
    ),
    OnRetry = args =>
    {
      Console.WriteLine($"[Flask] Retry attempt {args.AttemptNumber + 1}. " +
                            $"Reason: {args.Outcome.Exception?.Message ?? args.Outcome.Result?.StatusCode.ToString()}");
      return ValueTask.CompletedTask;
    }
  });

  // Layer 3: Circuit Breaker (innermost — tracks Flask health)
  // Opens when 50% of the last 3+ requests fail within 30 seconds.
  // Stays open for 30 seconds then tests with one request (half-open).
  pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
  {
    FailureRatio = 0.5,           // 50% failure rate triggers break
    SamplingDuration = TimeSpan.FromSeconds(30),
    MinimumThroughput = 3,         // need at least 3 requests to evaluate
    BreakDuration = TimeSpan.FromSeconds(30),
    OnOpened = args =>
    {
      Console.WriteLine(
              $"[FlaskCircuit] OPENED — Flask failing. " +
              $"Fast-failing requests for {args.BreakDuration.TotalSeconds}s. " +
              $"Last error: {args.Outcome.Exception?.Message ?? args.Outcome.Result?.StatusCode.ToString()}");
      return ValueTask.CompletedTask;
    },
    OnClosed = args =>
    {
      Console.WriteLine("[FlaskCircuit] CLOSED — Flask recovered. Normal operation resumed.");
      return ValueTask.CompletedTask;
    },
    OnHalfOpened = args =>
    {
      Console.WriteLine("[FlaskCircuit] HALF-OPEN — Testing Flask with one request.");
      return ValueTask.CompletedTask;
    }
  });
});

// ── Application Services ───────────────────────────────────────────────────────
builder.Services.AddSingleton<VtuCreditResolver>();
builder.Services.AddScoped<ISgpaService, SgpaService>();
builder.Services.AddScoped<IPdfExtractorService, PdfExtractorService>();

// ── CORS ───────────────────────────────────────────────────────────────────────
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>()
    ?? throw new InvalidOperationException("Cors:AllowedOrigins is not configured.");

builder.Services.AddCors(opt =>
    opt.AddPolicy("ReactApp", p =>
        p.WithOrigins(allowedOrigins)
         .AllowAnyHeader()
         .AllowAnyMethod()));

// ── File Upload Limits ─────────────────────────────────────────────────────────
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(opt =>
{
  opt.MultipartBodyLengthLimit = 2 * 1024 * 1024;
});

builder.WebHost.ConfigureKestrel(options =>
{
  options.Limits.MaxRequestBodySize = 2 * 1024 * 1024;
  options.Limits.MinRequestBodyDataRate = new Microsoft.AspNetCore.Server.Kestrel.Core.MinDataRate(
      bytesPerSecond: 100,
      gracePeriod: TimeSpan.FromSeconds(10));
});

// ── Response Compression ───────────────────────────────────────────────────────
builder.Services.AddResponseCompression(options =>
{
  options.EnableForHttps = false;
  options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
});

// ── ASPNETCORE_URLS set via environment ────────────────────────────────────────
// In Docker we set ASPNETCORE_URLS=http://+:8080
// Nginx handles HTTPS externally, ASP.NET only needs HTTP internally.

var app = builder.Build();

// ── Middleware Pipeline (ORDER MATTERS) ────────────────────────────────────────
app.UseMiddleware<KestrelSizeLimitMiddleware>();
app.UseMiddleware<ExceptionHandleMiddleware>();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseResponseCompression();
app.UseCors("ReactApp");

// No UseHttpsRedirection — Nginx handles this
// No UseHsts — Nginx handles HSTS header

app.UseAuthorization();

// Health check endpoint — used by Docker HEALTHCHECK and load balancers
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
  ResponseWriter = async (context, report) =>
  {
    context.Response.ContentType = "application/json";
    var result = System.Text.Json.JsonSerializer.Serialize(new
    {
      status = report.Status.ToString(),
      timestamp = DateTime.UtcNow,
      checks = report.Entries.Select(e => new
      {
        name = e.Key,
        status = e.Value.Status.ToString(),
        duration = e.Value.Duration.TotalMilliseconds
      })
    });
    await context.Response.WriteAsync(result);
  }
});

app.MapControllers();

// ── Run Migrations on Startup ──────────────────────────────────────────────────
// Runs in ALL environments. Fine for single-instance VPS with downtime deploys.
// For zero-downtime: run "dotnet ef database update" in your CI pipeline before
// starting the new container.
using (var scope = app.Services.CreateScope())
{
  var db = scope.ServiceProvider.GetRequiredService<SgpaDbContext>();
  var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
  try
  {
    logger.LogInformation("Applying PostgreSQL migrations...");
    db.Database.Migrate();
    logger.LogInformation("Migrations applied successfully.");
  }
  catch (Exception ex)
  {
    // Log but don't crash — allows readonly operations to still work
    // In production you may want to throw here so the container restarts
    logger.LogError(ex, "Failed to apply migrations on startup.");
    throw; // uncomment to make startup fail if DB is unreachable
  }
}

app.Run();
