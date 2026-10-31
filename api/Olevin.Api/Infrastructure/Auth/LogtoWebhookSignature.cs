using System.Security.Cryptography;
using System.Text;

namespace Olevin.Api.Infrastructure.Auth;

/// <summary>
/// Checks the signature that Logto puts in the <c>logto-signature-sha-256</c> header of a webhook request.
/// </summary>
public static class LogtoWebhookSignature
{
    /// <summary>
    /// The header that holds the signature.
    /// </summary>
    public const string HeaderName = "logto-signature-sha-256";

    /// <summary>
    /// Checks that the signature is the hex HMAC SHA-256 of the raw body made with the signing key.
    /// </summary>
    /// <param name="signingKey">The signing key of the webhook.</param>
    /// <param name="body">The raw body of the request.</param>
    /// <param name="signature">The value of the signature header, or <see langword="null"/> when it is missing.</param>
    /// <returns><see langword="true"/> when the signature matches the body.</returns>
    public static bool IsValid(string signingKey, ReadOnlySpan<byte> body, string? signature)
    {
        if (string.IsNullOrEmpty(signature))
            return false;

        byte[] expected = HMACSHA256.HashData(Encoding.UTF8.GetBytes(signingKey), body);

        try
        {
            return CryptographicOperations.FixedTimeEquals(
                expected,
                Convert.FromHexString(signature)
            );
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
