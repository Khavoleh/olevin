using System.Text.Json.Serialization;
using DotNetEnv;
using Npgsql;
using Olevin.Api.Features.Settings;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Infrastructure.Database;
using Olevin.Api.Infrastructure.Telemetry;
using Scalar.AspNetCore;
using Serilog;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.ErrorHandling;
using Wolverine.FluentValidation;
using Wolverine.Http;
using Wolverine.Http.FluentValidation;

Env.TraversePath().NoClobber().Load();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddTelemetry();

builder.Host.UseWolverine(options =>
{
    options.UseFluentValidation();
    options.UseEntityFrameworkCoreTransactions();
    options.Policies.AutoApplyTransactions();
    options
        .Policies.OnException<NpgsqlException>(exception => exception.IsTransient)
        .Or<TimeoutException>()
        .RetryWithCooldown(
            TimeSpan.FromMilliseconds(50),
            TimeSpan.FromMilliseconds(100),
            TimeSpan.FromMilliseconds(250)
        );
});
builder.Services.AddWolverineHttp();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter())
);

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddSettings();
builder.Services.AddLogtoAuthentication(builder.Configuration);
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
builder.Services.AddOpenApi();

WebApplication app = builder.Build();

app.UseExceptionHandler();
app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
    app.MapScalarApiReference(options =>
        {
            options.Title = "Olevin API";
            options.Theme = ScalarTheme.Kepler;
        })
        .AllowAnonymous();
}

app.MapHealthChecks("/health").AllowAnonymous();
app.MapWolverineEndpoints(options => options.UseFluentValidationProblemDetailMiddleware());

await app.RunAsync();
