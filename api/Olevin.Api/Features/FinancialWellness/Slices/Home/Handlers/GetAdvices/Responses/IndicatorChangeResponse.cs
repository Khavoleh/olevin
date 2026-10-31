namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Responses;

/// <summary>
/// The value of an indicator now and after the advice.
/// </summary>
/// <param name="From">The current value.</param>
/// <param name="To">The value after the advice.</param>
public sealed record IndicatorChangeResponse(double From, double To);
