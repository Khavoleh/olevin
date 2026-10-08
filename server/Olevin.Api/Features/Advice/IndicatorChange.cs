using Olevin.Api.Shared.Index.Indicators;

namespace Olevin.Api.Features.Advice;

/// <summary>
/// The change of one indicator by an advice.
/// </summary>
/// <param name="Indicator">The indicator.</param>
/// <param name="From">The current value.</param>
/// <param name="To">The value after the step.</param>
public sealed record IndicatorChange(Indicator Indicator, double From, double To);
