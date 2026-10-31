using System.Security.Cryptography;
using System.Text;
using Olevin.Api.Infrastructure.Auth;

namespace Olevin.Tests.Infrastructure.Auth;

public sealed class LogtoWebhookSignatureTests
{
    private const string Key = "NXudmFCGLx1ocjNYG2IchR84zeTJV7fc";

    private static readonly byte[] Body = Encoding.UTF8.GetBytes(
        """{"event":"User.Deleted","params":{"userId":"u_001"}}"""
    );

    private static string Sign(string key, byte[] body) =>
        Convert.ToHexStringLower(HMACSHA256.HashData(Encoding.UTF8.GetBytes(key), body));

    [Fact]
    public void Signature_OfTheBody_IsAccepted() =>
        Assert.True(LogtoWebhookSignature.IsValid(Key, Body, Sign(Key, Body)));

    [Fact]
    public void Signature_InUpperCase_IsAccepted() =>
        Assert.True(LogtoWebhookSignature.IsValid(Key, Body, Sign(Key, Body).ToUpperInvariant()));

    [Fact]
    public void Signature_OfAnotherKey_IsRejected() =>
        Assert.False(LogtoWebhookSignature.IsValid(Key, Body, Sign("another-key", Body)));

    [Fact]
    public void Signature_OfAnotherBody_IsRejected() =>
        Assert.False(
            LogtoWebhookSignature.IsValid(Key, Body, Sign(Key, Encoding.UTF8.GetBytes("{}")))
        );

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-hex")]
    [InlineData("abcd")]
    public void MissingOrMalformedSignature_IsRejected(string? signature) =>
        Assert.False(LogtoWebhookSignature.IsValid(Key, Body, signature));
}
