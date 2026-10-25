using Olevin.Api.Shared;

namespace Olevin.Api.Features.Settings.Slices.Profile.Handlers.UpdateBaseCurrency;

/// <summary>
/// The new base currency of a user.
/// </summary>
/// <param name="BaseCurrency">The base currency.</param>
public sealed record UpdateBaseCurrencyRequest(CurrencyCode BaseCurrency);
