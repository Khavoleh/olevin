using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Model;

namespace Olevin.Tests.Features.FinancialWellness.Helpers.Index.Model;

/// <summary>
/// A synthetic profile for calibration: indicators and the zone an expert expects for them.
/// </summary>
/// <param name="Name">What the profile describes.</param>
/// <param name="Indicators">SR, R, DTI, CV, RG.</param>
/// <param name="ExpectedZone">The zone expected without looking at the model.</param>
/// <param name="FuzzyIndex">I_fuzzy from an independent implementation of the model, for cross-checking.</param>
/// <param name="LinearIndex">I_lin from the same independent implementation.</param>
public sealed record SyntheticProfile(
    string Name,
    IndicatorVector Indicators,
    Zone ExpectedZone,
    double FuzzyIndex,
    double LinearIndex
)
{
    public override string ToString() => Name;
}
