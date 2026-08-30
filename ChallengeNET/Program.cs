using ClyvoCare.API.Data;
using ClyvoCare.API.Diagnostics;
using ClyvoCare.API.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configuração de Logging Estruturado com Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();

// Se não estiver no ambiente de testes, registra a conexão com o Oracle
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Health Checks (Liveness e Readiness)
builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(), tags: new[] { "liveness" })
    .AddCheck<OracleDbHealthCheck>("oracle_db", tags: new[] { "readiness" });

// OpenTelemetry (Tracing e Métricas)
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(TelemetryConfig.ServiceName))
    .WithTracing(tracing => tracing
        .AddSource(TelemetryConfig.ServiceName)
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddConsoleExporter())
    .WithMetrics(metrics => metrics
        .AddMeter(TelemetryConfig.ServiceName)
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddPrometheusExporter());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthorization();

// Endpoints de monitoramento
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("liveness")
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("readiness")
});

app.MapPrometheusScrapingEndpoint();
app.MapControllers();

app.Run();

public partial class Program { }