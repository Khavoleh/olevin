using FluentValidation;
using Olevin.Api.Shared.Index.Fuzzy;
using Olevin.Api.Shared.Index.Model;

namespace Olevin.Tests.Shared.Index.Fuzzy;

public sealed class MamdaniSystemTests
{
    private const double Precision = 1e-9;

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 25)]
    [InlineData(2, 50)]
    [InlineData(3, 75)]
    [InlineData(4, 100)]
    public void SingleOutputTerm_GivesItsPeak(int term, double expected)
    {
        foreach (double level in new[] { 0.1, 0.5, 1 })
        {
            double[] levels = new double[5];
            levels[term] = level;

            Assert.Equal(expected, OlevinModel.IndexSystem.Centroid(levels), Precision);
        }
    }

    [Fact]
    public void SingleStressTerm_GivesExactlyZero_AndSingleCalmTerm_GivesExactly100()
    {
        Assert.Equal(0, OlevinModel.IndexSystem.Centroid([1, 0, 0, 0, 0]), Precision);
        Assert.Equal(100, OlevinModel.IndexSystem.Centroid([0, 0, 0, 0, 1]), Precision);
    }

    [Fact]
    public void Centroid_MatchesASumWrittenOutByHand()
    {
        // "moderate" clipped at 0.833… and "strong" at 0.166… on [−50; 150], as in F₂ of the control example.
        double moderate = 5.0 / 6;
        double strong = 1.0 / 6;
        double numerator = 0;
        double denominator = 0;

        for (int y = -50; y <= 150; y++)
        {
            double mModerate = Math.Max(0, 1 - (Math.Abs(y - 50) / 50.0));
            double mStrong = Math.Max(0, 1 - (Math.Abs(y - 100) / 50.0));
            double mu = Math.Max(Math.Min(moderate, mModerate), Math.Min(strong, mStrong));
            numerator += y * mu;
            denominator += mu;
        }

        double centroid = OlevinModel.BufferSystem.Centroid([0, moderate, strong]);

        Assert.Equal(numerator / denominator, centroid, Precision);
        Assert.Equal(60.4, centroid, 0.1);
    }

    [Fact]
    public void Centroid_OfTwoEqualNeighbours_IsBetweenThem()
    {
        Assert.Equal(50, OlevinModel.CashFlowSystem.Centroid([0.5, 0, 0.5]), Precision);
        Assert.Equal(62.5, OlevinModel.IndexSystem.Centroid([0, 0, 1, 1, 0]), Precision);
    }

    [Fact]
    public void Centroid_WithoutActiveTerms_Throws()
    {
        Assert.Throws<InvalidOperationException>(() =>
            OlevinModel.IndexSystem.Centroid([0, 0, 0, 0, 0])
        );
    }

    [Fact]
    public void Evaluate_UsesProductForAndAndSumForRulesWithTheSameConclusion()
    {
        LinguisticVariable input = new(
            "x",
            0,
            10,
            [
                new("low", MembershipFunction.Trapezoid(0, 0, 2, 8)),
                new("high", MembershipFunction.Trapezoid(2, 8, 10, 10)),
            ]
        );
        LinguisticVariable output = new(
            "y",
            0,
            10,
            [
                new("low", MembershipFunction.Triangle(-10, 0, 10)),
                new("high", MembershipFunction.Triangle(0, 10, 20)),
            ]
        );
        MamdaniSystem system = new(
            "test",
            [input, input],
            output,
            [new(1, [0, 0], 0), new(2, [0, 1], 1), new(3, [1, 0], 1), new(4, [1, 1], 1)],
            -10,
            20
        );

        // x₁ = 5 → low 0.5, high 0.5; x₂ = 3.5 → low 0.75, high 0.25.
        MamdaniResult result = system.Evaluate(5, 3.5);

        Assert.Equal([0.375, 0.125, 0.375, 0.125], result.RuleStrengths.Select(r => r.Strength));
        Assert.Equal([0.375, 0.625], result.TermLevels);
        Assert.Equal(1, result.RuleStrengths.Sum(r => r.Strength), 1e-12);
        Assert.Equal(6.0141987829614605, result.Output, Precision);
    }

    [Fact]
    public void Evaluate_RejectsAWrongNumberOfInputs() =>
        Assert.Throws<ArgumentException>(() => OlevinModel.CashFlowSystem.Evaluate(0.1));

    [Theory]
    [InlineData(new[] { 3 }, 0, "Rule 1 refers to an unknown input term.")]
    [InlineData(new[] { -1 }, 0, "Rule 1 refers to an unknown input term.")]
    [InlineData(new[] { 0, 0 }, 0, "Rule 1 must have one condition per input.")]
    [InlineData(new[] { 0 }, 3, "Rule 1 refers to an unknown output term.")]
    public void Constructor_RejectsInvalidRules(int[] conditions, int conclusion, string message)
    {
        ValidationException exception = Assert.Throws<ValidationException>(() =>
            new MamdaniSystem(
                "bad",
                [OlevinModel.SavingsRate],
                OlevinModel.CashFlow,
                [new(1, conditions, conclusion)],
                -50,
                150
            )
        );

        Assert.Equal(message, Assert.Single(exception.Errors).ErrorMessage);
    }

    [Fact]
    public void Constructor_RejectsAGridWithoutWidth()
    {
        ValidationException exception = Assert.Throws<ValidationException>(() =>
            new MamdaniSystem(
                "bad",
                [OlevinModel.SavingsRate],
                OlevinModel.CashFlow,
                [new(1, [0], 0)],
                50,
                50
            )
        );

        Assert.Equal(nameof(MamdaniSystem.GridMin), Assert.Single(exception.Errors).PropertyName);
    }
}
