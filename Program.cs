// Program.cs — UPDATED
// ─────────────────────────────────────────────────────────────────────────────
// Changes from original:
//   1. Registered IPdfExtractorService → PdfExtractorService
//   2. Registered a named HttpClient "Flask" pointing to http://localhost:5050
//   3. Added camelCase JSON policy so Flask JSON deserialization works cleanly
//   4. Configured file upload size limits
//
// LEARNING CONCEPT — Named HttpClient:
//   Instead of creating HttpClient anywhere you need it (which causes socket
//   exhaustion), you register named clients once here.
//   Then inject IHttpClientFactory and call CreateClient("Flask").
//   The factory manages connection pooling and lifetime for you.
// ─────────────────────────────────────────────────────────────────────────────

using Microsoft.EntityFrameworkCore;
using SGPA_CALCULATOR.Application.Interface;
using SGPA_CALCULATOR.Application.Services;
using SGPA_CALCULATOR.Infrastructure.Data;
using SGPA_CALCULATOR.Middelware;
using SGPA_CALCULATOR.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ── JSON serialisation — camelCase policy ─────────────────────────────────────
// Flask returns camelCase JSON (Python convention).
// This tells System.Text.Json to match camelCase JSON keys to PascalCase C# props.
// File: Program.cs
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

// ── Database — Sem 1 & 2 seed data (21 rows) ─────────────────────────────────
builder.Services.AddDbContext<SgpaDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Named HttpClient for Flask microservice ────────────────────────────────────
//
// LEARNING: AddHttpClient("Flask", client => { ... })
//   This registers a factory that creates HttpClient instances pre-configured
//   with the base address.  In PdfExtractorService we call:
//       _httpFactory.CreateClient("Flask")
//   which gives us a client already pointed at Flask.




// Timeout: VTU PDFs extract in < 2 seconds on any modern machine.
// We set 30 seconds as a generous upper bound for slow hardware / large PDFs.


// ihttp client named as a flask, which manages the user
//and client related base adress and timeout already set

var flaskBaseUrl = builder.Configuration["FlaskService:BaseUrl"]
    ?? throw new InvalidOperationException(
        "FlaskService:BaseUrl is not configured. " +
        "Add it to appsettings.json or set FLASKSERVICE__BASEURL environment variable.");

// GetValue<int> with default 30:
//   If FlaskService:TimeoutSeconds is missing → use 30, no crash
//   Why different from above? Because missing timeout has a safe fallback.
//   Missing URL has NO safe fallback — we can't guess where Flask is.
var flaskTimeout = builder.Configuration.GetValue<int>("FlaskService:TimeoutSeconds", 30);

builder.Services.AddHttpClient("Flask", client =>
{
    client.BaseAddress = new Uri(flaskBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(flaskTimeout);
});


// ── Application services ───────────────────────────────────────────────────────
// Scoped = new instance per HTTP request (safe with DbContext which is also scoped)
builder.Services.AddScoped<VtuCreditResolver>();
builder.Services.AddScoped<ISgpaService, SgpaService>();
builder.Services.AddScoped<IPdfExtractorService, PdfExtractorService>();


// ── CORS — allow React dev server ────────────────────────────────────────────

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


// ── File upload size ───────────────────────────────────────────────────────────
// Default ASP.NET limit is 30 MB — we set it explicitly for clarity.
// VTU PDFs are typically 100–400 KB so 10 MB is plenty.


builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(opt =>
{
    opt.MultipartBodyLengthLimit = 2 * 1024 * 1024; // 2 MB
});



builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 2 * 1024 * 1024; // 2 MB — hard limit


    options.Limits.MinRequestBodyDataRate = new Microsoft.AspNetCore.Server.Kestrel.Core.MinDataRate(
        bytesPerSecond: 100,       // minimum 100 bytes/second upload speed
        gracePeriod: TimeSpan.FromSeconds(10));
});



var app = builder.Build();




//first middlware to catch error

app.UseMiddleware<KestrelSizeLimitMiddleware>();
app.UseMiddleware<ExceptionHandleMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ReactApp");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// ── Auto-migrate on startup (dev only) ───────────────────────────────────────
// This creates the SubjectMasters table with 21 seed rows if it doesn't exist.
// Remove this in production — run migrations manually with:
//   dotnet ef database update
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<SgpaDbContext>();
    db.Database.Migrate();
}

app.Run();