using Keystone.Api;
using Keystone.Application;
using Keystone.Application.Customers.Commands.RegisterCustomer;
using Keystone.Application.Customers.Queries.GetCustomer;
using Keystone.Infrastructure;
using Keystone.Infrastructure.Persistence;
using MediatR;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("Default") ?? "Data Source=keystone.db");

builder.Services.AddHealthChecks();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var otel = builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("Keystone.Api"))
    .WithTracing(tracing => tracing.AddAspNetCoreInstrumentation())
    .WithMetrics(metrics => metrics.AddAspNetCoreInstrumentation());

// Export only when a collector is configured, so local runs stay quiet.
if (!string.IsNullOrEmpty(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
{
    otel.UseOtlpExporter();
}

var app = builder.Build();

app.UseExceptionHandler();
app.MapHealthChecks("/health");

var customers = app.MapGroup("/customers");

customers.MapPost("/", async (RegisterCustomerCommand command, ISender sender, CancellationToken cancellationToken) =>
{
    var result = await sender.Send(command, cancellationToken);
    return result.IsSuccess
        ? Results.Created($"/customers/{result.Value}", new { id = result.Value })
        : result.ToProblem();
});

customers.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
{
    var result = await sender.Send(new GetCustomerQuery(id), cancellationToken);
    return result.IsSuccess ? Results.Ok(result.Value) : result.ToProblem();
});

// Development convenience: create the schema on startup. Use migrations in production.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.EnsureCreatedAsync();
}

await app.RunAsync();

// Exposed so the test project can drive the app with WebApplicationFactory.
public partial class Program;
