namespace Olevin.Api.Features.Snapshots.Currency;

/// <summary>
/// A currency in which amounts can be entered.
/// </summary>
public enum CurrencyCode
{
    /// <summary>
    /// Ukrainian hryvnia; NBU rates are quoted against it.
    /// </summary>
    UAH,

    /// <summary>
    /// US dollar.
    /// </summary>
    USD,

    /// <summary>
    /// Euro.
    /// </summary>
    EUR,

    /// <summary>
    /// Polish złoty.
    /// </summary>
    PLN,

    /// <summary>
    /// Canadian dollar.
    /// </summary>
    CAD,
}
