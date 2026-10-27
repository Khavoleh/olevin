using System.Text.RegularExpressions;
using FluentValidation;

namespace Olevin.Api.Features.Settings.Slices.Profile.Handlers.SubscribePush;

/// <summary>
/// Validates the push subscription of a browser.
/// </summary>
public sealed partial class SubscribePushValidator : AbstractValidator<SubscribePushRequest>
{
    private const int MaxP256dhLength = 128;
    private const int MaxAuthLength = 64;
    private const int MaxDeviceNameLength = 128;

    [GeneratedRegex("^[A-Za-z0-9_-]+={0,2}$(?!\n)")]
    private static partial Regex Base64Url();

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscribePushValidator"/> class.
    /// </summary>
    public SubscribePushValidator()
    {
        RuleFor(request => request.Endpoint).Endpoint();
        RuleFor(request => request.P256dh)
            .NotEmpty()
            .WithMessage("P256dh key is required.")
            .MaximumLength(MaxP256dhLength)
            .WithMessage($"P256dh key cannot be longer than {MaxP256dhLength} characters.")
            .Matches(Base64Url())
            .WithMessage("P256dh key must be base64url.");
        RuleFor(request => request.Auth)
            .NotEmpty()
            .WithMessage("Auth key is required.")
            .MaximumLength(MaxAuthLength)
            .WithMessage($"Auth key cannot be longer than {MaxAuthLength} characters.")
            .Matches(Base64Url())
            .WithMessage("Auth key must be base64url.");
        RuleFor(request => request.DeviceName)
            .NotEmpty()
            .WithMessage("Device name is required.")
            .MaximumLength(MaxDeviceNameLength)
            .WithMessage($"Device name cannot be longer than {MaxDeviceNameLength} characters.");
    }
}
