using Olevin.Api.Shared.Index.Fuzzy;
using Olevin.Api.Shared.Index.Indicators;
using Olevin.Api.Shared.Index.Model;

namespace Olevin.Tests.Shared.Index.Model;

/// <summary>
/// The reference profiles of the brief; the precision is 0.1.
/// </summary>
public sealed class ReferenceProfilesTests
{
    private const double Precision = 0.1;

    public static TheoryData<string, IndicatorVector, double, double, double, double> Profiles =>
        new()
        {
            { "best", new(0.4, 9, 0, 0, 1), 100, 100, 100, 100 },
            { "worst", new(-0.2, 0, 0.6, 0.5, 0), 0, 0, 0, 0 },
            { "A", new(-0.10, 6, 0, 0.05, 1.0 / 6), 0, 100, 25.0, 60.0 },
            { "Б", new(0.10, 3.5, 0.30, 0.20, 0.5), 50.0, 60.4, 56.2, 49.8 },
        };

    [Theory]
    [MemberData(nameof(Profiles))]
    public void Profile_GivesTheReferenceValues(
        string name,
        IndicatorVector indicators,
        double cashFlow,
        double buffer,
        double fuzzyIndex,
        double linearIndex
    )
    {
        FuzzyEvaluation result = OlevinModel.Evaluate(indicators);

        Assert.True(name.Length > 0);
        Assert.Equal(cashFlow, result.CashFlow.Output, Precision);
        Assert.Equal(buffer, result.Buffer.Output, Precision);
        Assert.Equal(fuzzyIndex, result.Value, Precision);
        Assert.Equal(linearIndex, LinearIndex.Calculate(indicators), Precision);
    }

    [Fact]
    public void ProfileB_FiresTheRulesOfTheControlExample()
    {
        FuzzyEvaluation result = OlevinModel.Evaluate(
            new IndicatorVector(0.10, 3.5, 0.30, 0.20, 0.5)
        );

        // F₁: SR low 1; DTI low 0.375, moderate 0.625.
        Assert.Equal([0, 1, 0], result.CashFlow.InputDegrees[0], new Tolerance(1e-9));
        Assert.Equal([0.375, 0.625, 0], result.CashFlow.InputDegrees[1], new Tolerance(1e-9));
        Assert.Equal([(4, 0.375), (5, 0.625)], Fired(result.CashFlow));
        Assert.Equal(50.0, result.CashFlow.Output, 1e-9);

        // F₂: R sufficient 0.833, large 0.167; CV moderate 1.
        Assert.Equal([0, 5.0 / 6, 1.0 / 6], result.Buffer.InputDegrees[0], new Tolerance(1e-9));
        Assert.Equal([0, 1, 0], result.Buffer.InputDegrees[1], new Tolerance(1e-9));
        Assert.Equal([(14, 0.833), (17, 0.167)], Fired(result.Buffer));
        Assert.Equal(60.4, result.Buffer.Output, Precision);

        // F₃: flow moderate 1; buffer moderate 0.793, strong 0.207; RG sometimes 1.
        Assert.Equal([0, 1, 0], result.Index.InputDegrees[0], new Tolerance(1e-9));
        Assert.Equal([0, 0.793, 0.207], result.Index.InputDegrees[1], new Tolerance(1e-3));
        Assert.Equal([0, 1, 0], result.Index.InputDegrees[2], new Tolerance(1e-9));
        Assert.Equal([(32, 0.793), (35, 0.207)], Fired(result.Index));
        Assert.Equal(56.2, result.Value, Precision);
    }

    [Fact]
    public void ProfileB_WithSelfRating4_Gives58Point7InTheUnstableZone()
    {
        IndexResult result = IndexCalculator.Calculate(
            new IndicatorVector(0.10, 3.5, 0.30, 0.20, 0.5),
            4,
            6
        );

        Assert.Equal(58.7, result.Final, Precision);
        Assert.Equal(Zone.Unstable, result.Zone);
        Assert.False(result.IsPreliminary);
    }

    private static (int Number, double Strength)[] Fired(MamdaniResult result)
    {
        return
        [
            .. result
                .RuleStrengths.Where(rule => rule.Strength > 1e-9)
                .Select(rule => (rule.Number, Math.Round(rule.Strength, 3))),
        ];
    }

    private sealed class Tolerance(double precision) : IEqualityComparer<double>
    {
        public bool Equals(double x, double y) => Math.Abs(x - y) <= precision;

        public int GetHashCode(double obj) => 0;
    }
}
