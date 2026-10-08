using System.Text.Json.Serialization;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Olevin.Api.Infrastructure;
using Olevin.Api.Infrastructure.Auth;
using Olevin.Api.Infrastructure.Telemetry;
using Scalar.AspNetCore;
using Serilog;
using Wolverine;
using Wolverine.FluentValidation;
using Wolverine.Http;
using Wolverine.Http.FluentValidation;

Env.TraversePath().NoClobber().Load();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddTelemetry();

builder.Host.UseWolverine(options => options.UseFluentValidation());
builder.Services.AddWolverineHttp();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter())
);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Olevin"))
);

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
