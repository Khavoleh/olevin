using System.Text.RegularExpressions;
using FluentValidation;
using Olevin.Api.Features.Settings.Data.UserSettings;
using Olevin.Api.Shared;

namespace Olevin.Api.Features.Settings.Validators;

/// <summary>
/// The rules of the user settings, shared by every request that sets them.
/// </summary>
public static partial class UserSettingRules
{
    private const int MaxTimezoneLength = 64;

    [GeneratedRegex(@"^[A-Za-z0-9_+-]+(?:/[A-Za-z0-9_+-]+)*$(?!\n)")]
    private static partial Regex TimezoneFormat();

    /// <summary>
    /// Requires a supported base currency.
    /// </summary>
    /// <typeparam name="T">The validated request.</typeparam>
    /// <param name="rule">The rule of the base currency.</param>
    /// <returns>The rule.</returns>
    public static IRuleBuilderOptions<T, CurrencyCode> BaseCurrency<T>(
        this IRuleBuilder<T, CurrencyCode> rule
    ) => rule.NotNull().IsInEnum().WithMessage("Currency is not supported.");

    /// <summary>
    /// Requires a supported interface language.
    /// </summary>
    /// <typeparam name="T">The validated request.</typeparam>
    /// <param name="rule">The rule of the interface language.</param>
    /// <returns>The rule.</returns>
    public static IRuleBuilderOptions<T, LocaleCode> Locale<T>(
        this IRuleBuilder<T, LocaleCode> rule
    ) => rule.NotNull().IsInEnum().WithMessage("Locale is not supported.");

    /// <summary>
    /// Requires a day of the month from 1 to 31.
    /// </summary>
    /// <typeparam name="T">The validated request.</typeparam>
    /// <param name="rule">The rule of the snapshot day.</param>
    /// <returns>The rule.</returns>
    public static IRuleBuilderOptions<T, short> SnapshotDay<T>(this IRuleBuilder<T, short> rule) =>
        rule.NotNull()
            .InclusiveBetween((short)1, (short)31)
            .WithMessage("Snapshot day must be from 1 to 31.");

    /// <summary>
    /// Requires a known IANA time zone name that fits the column.
    /// </summary>
    /// <typeparam name="T">The validated request.</typeparam>
    /// <param name="rule">The rule of the time zone.</param>
    /// <returns>The rule.</returns>
    public static IRuleBuilderOptions<T, string> Timezone<T>(this IRuleBuilder<T, string> rule) =>
        rule.NotEmpty()
            .WithMessage("Timezone is required.")
            .MaximumLength(MaxTimezoneLength)
            .WithMessage($"Timezone cannot be longer than {MaxTimezoneLength} characters.")
            .Matches(TimezoneFormat())
            .WithMessage("Timezone must be an IANA name, such as Europe/Kyiv.")
            .Must(timezone => TimeZoneInfo.TryFindSystemTimeZoneById(timezone, out _))
            .WithMessage("Timezone is not known.");
}
