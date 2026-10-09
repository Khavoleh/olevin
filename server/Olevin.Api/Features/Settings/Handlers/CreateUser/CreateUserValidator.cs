using System.Text.RegularExpressions;
using FluentValidation;

namespace Olevin.Api.Features.Settings.Handlers.CreateUser;

/// <summary>
/// Validates the settings of a new user.
/// </summary>
public sealed partial class CreateUserValidator : AbstractValidator<CreateUserRequest>
{
    private const int MaxTimezoneLength = 64;

    [GeneratedRegex(@"^[A-Za-z0-9_+-]+(/[A-Za-z0-9_+-]+)*\z")]
    private static partial Regex TimezoneFormat();

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateUserValidator"/> class.
    /// </summary>
    public CreateUserValidator()
    {
        RuleFor(request => request.BaseCurrency)
            .IsInEnum()
            .WithMessage("Currency is not supported.");

        RuleFor(request => request.Locale).IsInEnum().WithMessage("Locale is not supported.");

        RuleFor(request => request.SnapshotDay)
            .InclusiveBetween((short)1, (short)31)
            .WithMessage("Snapshot day must be from 1 to 31.");

        RuleFor(request => request.Timezone)
            .NotEmpty()
            .WithMessage("Timezone is required.")
            .MaximumLength(MaxTimezoneLength)
            .WithMessage($"Timezone cannot be longer than {MaxTimezoneLength} characters.")
            .Matches(TimezoneFormat())
            .WithMessage("Timezone must be an IANA name, such as Europe/Kyiv.")
            .Must(timezone => TimeZoneInfo.TryFindSystemTimeZoneById(timezone, out _))
            .WithMessage("Timezone is not known.");
    }
}
