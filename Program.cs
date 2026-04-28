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

var builder = WebApplication.CreateBuilder(args);

// ── JSON serialisation — camelCase policy ─────────────────────────────────────
// Flask returns camelCase JSON (Python convention).
// This tells System.Text.Json to match camelCase JSON keys to PascalCase C# props.
builder.Services.AddControllers()
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
//
// Timeout: VTU PDFs extract in < 2 seconds on any modern machine.
// We set 30 seconds as a generous upper bound for slow hardware / large PDFs.
builder.Services.AddHttpClient("Flask", client =>
{
    client.BaseAddress = new Uri("http://localhost:5050");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// ── Application services ───────────────────────────────────────────────────────
// Scoped = new instance per HTTP request (safe with DbContext which is also scoped)
builder.Services.AddScoped<VtuCreditResolver>();
builder.Services.AddScoped<ISgpaService, SgpaService>();
builder.Services.AddScoped<IPdfExtractorService, PdfExtractorService>();

// ── File upload size ───────────────────────────────────────────────────────────
// Default ASP.NET limit is 30 MB — we set it explicitly for clarity.
// VTU PDFs are typically 100–400 KB so 10 MB is plenty.
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(opt =>
{
    opt.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB
});

// ── CORS — allow React dev server ────────────────────────────────────────────
builder.Services.AddCors(opt =>
    opt.AddPolicy("ReactApp", p =>
        p.WithOrigins("http://localhost:3000", "http://localhost:5173")
         .AllowAnyHeader()
         .AllowAnyMethod()));

var app = builder.Build();

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