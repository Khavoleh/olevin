namespace Olevin.Api.Features.Settings.Slices.Profile.Handlers.UpdateTimezone;

/// <summary>
/// The new time zone of a user.
/// </summary>
/// <param name="Timezone">The IANA time zone name.</param>
public sealed record UpdateTimezoneRequest(string Timezone);
