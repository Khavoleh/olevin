using System.Buffers.Text;
using Lib.Net.Http.WebPush;
using Lib.Net.Http.WebPush.Authentication;
using Microsoft.Extensions.Options;

namespace Olevin.Api.Infrastructure.WebPush;

/// <summary>
/// Registers the Web Push client.
/// </summary>
public static class WebPushServices
{
    private const int PublicKeyLength = 65;
    private const int PrivateKeyLength = 32;

    /// <summary>
    /// Lets the long-lived client open new connections now and then, so it follows DNS changes of the push services.
    /// </summary>
    private static readonly TimeSpan ConnectionLifetime = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Fills <see cref="WebPushOptions"/> from the <c>WebPush</c> section, checks it when the application starts, and
    /// adds one <see cref="PushServiceClient"/> signed with the VAPID keys. It is a singleton over its own
    /// <see cref="HttpClient"/>, which Wolverine can inject into the handlers and which logs no request, because the
    /// subscription endpoint is a secret URL.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddWebPush(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddOptions<WebPushOptions>()
            .Bind(configuration.GetRequiredSection(WebPushOptions.SectionName))
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Subject),
                "The Web Push subject is missing."
            )
            .Validate(
                options => IsKey(options.PublicKey, PublicKeyLength),
                "The Web Push public key must be an uncompressed P-256 point in base64url."
            )
            .Validate(
                options => IsKey(options.PrivateKey, PrivateKeyLength),
                "The Web Push private key must be a P-256 scalar in base64url."
            )
            .ValidateOnStart();

        services.AddSingleton(provider =>
        {
            WebPushOptions options = provider.GetRequiredService<IOptions<WebPushOptions>>().Value;

            return new PushServiceClient(
                new HttpClient(
                    new SocketsHttpHandler { PooledConnectionLifetime = ConnectionLifetime }
                )
            )
            {
                DefaultAuthentication = new VapidAuthentication(
                    options.PublicKey,
                    options.PrivateKey
                )
                {
                    Subject = options.Subject,
                },
            };
        });

        return services;
    }

    private static bool IsKey(string key, int length)
    {
        try
        {
            return Base64Url.DecodeFromChars(key).Length == length;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
