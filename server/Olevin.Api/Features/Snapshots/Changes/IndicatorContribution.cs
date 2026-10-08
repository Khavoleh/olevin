using Olevin.Api.Shared.Index.Indicators;

namespace Olevin.Api.Features.Snapshots.Changes;

/// <summary>
/// The contribution of one indicator to the change of I_fuzzy.
/// </summary>
/// <param name="Indicator">The indicator.</param>
/// <param name="Contribution">C_k in points.</param>
public sealed record IndicatorContribution(Indicator Indicator, double Contribution);
