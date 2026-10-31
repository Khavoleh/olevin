using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;

namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Models;

/// <summary>
/// The change of one indicator by an advice.
/// </summary>
/// <param name="Indicator">The indicator.</param>
/// <param name="From">The current value.</param>
/// <param name="To">The value after the step.</param>
public sealed record IndicatorChange(Indicator Indicator, double From, double To);
