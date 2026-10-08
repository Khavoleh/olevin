using Olevin.Api.Shared.Index.Fuzzy;
using Olevin.Api.Shared.Index.Indicators;
using Olevin.Api.Shared.Index.Model;

namespace Olevin.Tests.Shared.Index.Model;

/// <summary>
/// Calibration of the model on the synthetic profiles: expected zones and monotonicity.
/// </summary>
public sealed class CalibrationTests
{
    /// <summary>
    /// Rounding noise allowed when comparing the index of neighbouring points.
    /// </summary>
    private const double Noise = 1e-9;

    private static readonly Dictionary<Indicator, double[]> Grid = new()
    {
        [Indicator.SavingsRate] = [-0.2, -0.05, 0.05, 0.1, 0.15, 0.25, 0.4],
        [Indicator.Reserve] = [0, 1, 2, 3, 4.5, 6, 9],
        [Indicator.DebtToIncome] = [0, 0.2, 0.3, 0.36, 0.4, 0.43, 0.6],
        [Indicator.ExpenseVariation] = [0, 0.1, 0.15, 0.2, 0.25, 0.3, 0.5],
        [Indicator.SavingsRegularity] = [0, 1.0 / 6, 2.0 / 6, 0.5, 4.0 / 6, 5.0 / 6, 1],
    };

    public static TheoryData<SyntheticProfile> Profiles => [.. SyntheticProfiles.All];

    [Fact]
    public void Profiles_AreFifteen_AndIncludeTheReferenceOnes()
    {
        Assert.Equal(15, SyntheticProfiles.All.Count);
        Assert.Contains(
            SyntheticProfiles.All,
            p => p.Indicators == new IndicatorVector(0.4, 9, 0, 0, 1)
        );
        Assert.Contains(
            SyntheticProfiles.All,
            p => p.Indicators == new IndicatorVector(-0.2, 0, 0.6, 0.5, 0)
        );
        Assert.Contains(SyntheticProfiles.All, p => p.Name == "Профіль А");
        Assert.Contains(SyntheticProfiles.All, p => p.Name == "Профіль Б");
    }

    [Theory]
    [MemberData(nameof(Profiles))]
    public void Profile_FallsIntoTheExpectedZone(SyntheticProfile profile)
    {
        double fuzzyIndex = OlevinModel.Evaluate(profile.Indicators).Value;

        Assert.Equal(profile.ExpectedZone, Zones.Of(fuzzyIndex));
    }

    [Theory]
    [MemberData(nameof(Profiles))]
    public void Profile_MatchesTheIndependentImplementation(SyntheticProfile profile)
    {
        Assert.Equal(profile.FuzzyIndex, OlevinModel.Evaluate(profile.Indicators).Value, 0.01);
        Assert.Equal(profile.LinearIndex, LinearIndex.Calculate(profile.Indicators), 0.01);
    }

    [Fact]
    public void Index_StaysWithin0And100()
    {
        foreach (IndicatorVector point in GridPoints())
        {
            FuzzyEvaluation result = OlevinModel.Evaluate(point);

            Assert.InRange(result.Value, 0, 100);
            Assert.InRange(result.CashFlow.Output, 0, 100);
            Assert.InRange(result.Buffer.Output, 0, 100);
        }
    }

    public static TheoryData<Indicator> Indicators => [.. Enum.GetValues<Indicator>()];

    [Theory]
    [MemberData(nameof(Indicators))]
    public void ImprovingOneIndicator_NeverLowersTheIndex(Indicator indicator)
    {
        LinguisticVariable variable = OlevinModel.Variable(indicator);
        bool lowerIsBetter = indicator is Indicator.DebtToIncome or Indicator.ExpenseVariation;
        const int steps = 60;
        double largestDip = 0;

        foreach (IndicatorVector point in GridPoints().DistinctBy(p => p.With(indicator, 0)))
        {
            double best = double.MinValue;

            // Walk from the worst to the best value; the dip is the largest fall below the best value so far.
            for (int i = 0; i <= steps; i++)
            {
                double share = (double)i / steps;
                double value = lowerIsBetter
                    ? variable.Max - ((variable.Max - variable.Min) * share)
                    : variable.Min + ((variable.Max - variable.Min) * share);
                double index = OlevinModel.Evaluate(point.With(indicator, value)).Value;

                best = Math.Max(best, index);
                largestDip = Math.Max(largestDip, best - index);
            }
        }

        Assert.InRange(largestDip, 0, Noise);
    }

    [Fact]
    public void FormerDip_MoreStableExpensesNowRaiseTheBuffer()
    {
        // With min and max, CV 0.15 → 0.10 lowered the buffer from 39.03 to 33.50 and I_fuzzy by 2.51: the rules
        // "small reserve → weak" and "sufficient reserve → moderate" lost their middle term at once. With the product
        // and the sum the confidence only moves between the conclusions, so the buffer grows.
        IndicatorVector before = new(0.23, 1.54, 0.10, 0.15, 0.06);
        IndicatorVector after = before with { ExpenseVariation = 0.10 };

        FuzzyEvaluation from = OlevinModel.Evaluate(before);
        FuzzyEvaluation to = OlevinModel.Evaluate(after);

        Assert.Equal(21.79, from.Buffer.Output, 0.01);
        Assert.Equal(33.50, to.Buffer.Output, 0.01);
        Assert.Equal(4.77, to.Value - from.Value, 0.01);
    }

    [Fact]
    public void RuleStrengths_SumTo1_InEverySystem()
    {
        foreach (IndicatorVector point in GridPoints())
        {
            FuzzyEvaluation result = OlevinModel.Evaluate(point);

            Assert.All(
                [result.CashFlow, result.Buffer, result.Index],
                system => Assert.Equal(1, system.RuleStrengths.Sum(r => r.Strength), 1e-9)
            );
        }
    }

    [Fact]
    public void ImprovingEveryIndicator_FromWorstToBest_RaisesTheIndexFrom0To100()
    {
        IndicatorVector worst = new(-0.2, 0, 0.6, 0.5, 0);
        IndicatorVector best = new(0.4, 9, 0, 0, 1);

        Assert.Equal(0, OlevinModel.Evaluate(worst).Value, 1e-9);
        Assert.Equal(100, OlevinModel.Evaluate(best).Value, 1e-9);
    }

    private static IEnumerable<IndicatorVector> GridPoints()
    {
        foreach (double sr in Grid[Indicator.SavingsRate])
        {
            foreach (double r in Grid[Indicator.Reserve])
            {
                foreach (double dti in Grid[Indicator.DebtToIncome])
                {
                    foreach (double cv in Grid[Indicator.ExpenseVariation])
                    {
                        foreach (double rg in Grid[Indicator.SavingsRegularity])
                            yield return new IndicatorVector(sr, r, dti, cv, rg);
                    }
                }
            }
        }
    }
}
