using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Olevin.Api.Infrastructure.Auth;

/// <summary>
/// Authentication with Logto access tokens.
/// </summary>
public static class Auth
{
    /// <summary>
    /// The claim that holds the Logto user identifier.
    /// </summary>
    private const string SubjectClaim = "sub";

    /// <summary>
    /// Validates Logto access tokens issued for the API resource and requires a signed-in user on every endpoint
    /// unless it explicitly allows anonymous access.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration with the <c>Logto</c> section.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddLogtoAuthentication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        LogtoOptions logto =
            configuration.GetRequiredSection(LogtoOptions.SectionName).Get<LogtoOptions>()
            ?? throw new InvalidOperationException("Logto settings are missing.");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = new Uri(logto.Endpoint, "oidc").ToString();
                options.Audience = logto.ApiResource;
                options.MapInboundClaims = false;
                options.TokenValidationParameters.NameClaimType = SubjectClaim;
                options.TokenValidationParameters.ValidTypes = ["at+jwt"];
            });

        services
            .AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

        return services;
    }

    /// <summary>
    /// Gets the Logto user identifier of an authenticated user.
    /// </summary>
    /// <param name="user">The authenticated user.</param>
    /// <returns>The value of the <c>sub</c> claim.</returns>
    public static string GetSubject(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(SubjectClaim)
            ?? throw new InvalidOperationException("The access token has no subject.");
    }
}
