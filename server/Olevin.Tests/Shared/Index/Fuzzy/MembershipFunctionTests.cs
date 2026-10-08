using Olevin.Api.Shared.Index.Fuzzy;
using Olevin.Api.Shared.Index.Model;

namespace Olevin.Tests.Shared.Index.Fuzzy;

public sealed class MembershipFunctionTests
{
    private const double Precision = 1e-9;

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0.05, 0.5)]
    [InlineData(0.1, 1)]
    [InlineData(0.15, 0.5)]
    [InlineData(0.2, 0)]
    [InlineData(-1, 0)]
    [InlineData(1, 0)]
    public void Triangle_RisesToThePeakAndFallsBack(double x, double expected)
    {
        MembershipFunction low = MembershipFunction.Triangle(0, 0.1, 0.2);

        Assert.Equal(expected, low.Evaluate(x), Precision);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(2, 0.5)]
    [InlineData(3, 1)]
    [InlineData(6, 1)]
    [InlineData(7.5, 0.5)]
    [InlineData(9, 0)]
    public void Trapezoid_HasAPlateauBetweenBAndC(double x, double expected)
    {
        MembershipFunction trapezoid = MembershipFunction.Trapezoid(1, 3, 6, 9);

        Assert.Equal(expected, trapezoid.Evaluate(x), Precision);
    }

    [Theory]
    [InlineData(-0.2, 1)]
    [InlineData(-0.5, 1)]
    [InlineData(0, 1)]
    [InlineData(0.05, 0.5)]
    [InlineData(0.1, 0)]
    public void LeftEdgeTerm_WithAEqualToB_IsFullyTrueUpToTheEdge(double x, double expected)
    {
        MembershipFunction negative = MembershipFunction.Trapezoid(-0.2, -0.2, 0, 0.1);

        Assert.Equal(expected, negative.Evaluate(x), Precision);
    }

    [Theory]
    [InlineData(0.6, 1)]
    [InlineData(0.9, 1)]
    [InlineData(0.43, 1)]
    [InlineData(0.36, 0)]
    public void RightEdgeTerm_WithCEqualToD_IsFullyTrueUpToTheEdge(double x, double expected)
    {
        MembershipFunction high = MembershipFunction.Trapezoid(0.36, 0.43, 0.6, 0.6);

        Assert.Equal(expected, high.Evaluate(x), Precision);
    }

    [Fact]
    public void DebtToIncome_Of30Percent_IsPartlyLowAndPartlyModerate()
    {
        double[] degrees = OlevinModel.DebtToIncome.Fuzzify(0.3);

        Assert.Equal(0.375, degrees[0], Precision);
        Assert.Equal(0.625, degrees[1], Precision);
        Assert.Equal(0, degrees[2], Precision);
    }

    [Fact]
    public void Fuzzify_ClampsTheValueToTheRange()
    {
        Assert.Equal(OlevinModel.SavingsRate.Fuzzify(-0.2), OlevinModel.SavingsRate.Fuzzify(-0.35));
        Assert.Equal(OlevinModel.Reserve.Fuzzify(9), OlevinModel.Reserve.Fuzzify(24));
    }

    public static TheoryData<string> InputVariables => ["SR", "R", "DTI", "CV", "RG"];

    [Theory]
    [MemberData(nameof(InputVariables))]
    public void InputTerms_CoverTheWholeRangeAndAddUpToOne(string name)
    {
        LinguisticVariable variable = new[]
        {
            OlevinModel.SavingsRate,
            OlevinModel.Reserve,
            OlevinModel.DebtToIncome,
            OlevinModel.ExpenseVariation,
            OlevinModel.SavingsRegularity,
        }.Single(v => v.Name == name);

        for (int i = 0; i <= 1000; i++)
        {
            double x = variable.Min + ((variable.Max - variable.Min) * i / 1000);

            Assert.Equal(1, variable.Fuzzify(x).Sum(), Precision);
        }
    }
}
