using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using Keystone.Api.Common;
using Keystone.Api.Customers.Endpoints;
using Keystone.Api.Customers.Repositories;
using Keystone.Api.Customers.Services;
using Keystone.Api.Customers.Services.Interface;
using Keystone.Api.Customers.Validators;
using Keystone.Api.Infrastructure.Data;
using Keystone.Contracts.Customers;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using SerilogTracing;

// Startup logger until UseSerilog replaces it. Not CreateBootstrapLogger: a ReloadableLogger freezes once
// per process, so parallel WebApplicationFactory hosts would throw "The logger is already frozen".
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

using var tracing = new ActivityListenerConfiguration()
    .Instrument.AspNetCoreRequests()
    .TraceToSharedLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, logger) => logger
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services));

    builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();
    builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = true);

    ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;

    RegisterDependencies(builder.Services);

    var app = builder.Build();

    app.UseExceptionHandler();
    app.UseSerilogRequestLogging();
    app.UseAuthentication();
    app.UseAuthorization();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi().AllowAnonymous();
    }

    app.MapHealthChecks("/health").AllowAnonymous();

    app.MapVersionEndpoints()
        .MapGroup("v1")
        .MapCustomerEndpoints();

    await app.RunAsync();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    throw;
}
finally
{
    await Log.CloseAndFlushAsync();
}

static void RegisterDependencies(IServiceCollection services)
{
    services.AddSingleton(TimeProvider.System);

    services.AddTransient<IValidator<RegisterCustomerRequest>, RegisterCustomerRequestValidator>();

    services.AddSingleton<ICustomerRepository, CustomerRepository>();
    services.AddSingleton<ICustomerService, CustomerService>();

    services.AddPostgres();

    // MapInboundClaims=false keeps "sub" and "scope" as issued instead of renaming them.
    services.AddAuthentication().AddJwtBearer(options => options.MapInboundClaims = false);
    // The fallback policy is secure by default: a route without RequireAuthorization still needs a signed-in caller.
    services.AddAuthorizationBuilder()
        .AddRolePolicies()
        .SetFallbackPolicy(new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build());
}

// Exposed so the test project can host the API with WebApplicationFactory.
public partial class Program;
