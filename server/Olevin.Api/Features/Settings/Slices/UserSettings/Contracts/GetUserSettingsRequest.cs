namespace Olevin.Api.Features.Settings.Slices.UserSettings.Contracts;

/// <summary>
/// Asks for the user signed in with <paramref name="AuthSub"/>, together with their settings.
/// </summary>
/// <param name="AuthSub">The subject of the user's access token.</param>
public sealed record GetUserSettingsRequest(string AuthSub);
