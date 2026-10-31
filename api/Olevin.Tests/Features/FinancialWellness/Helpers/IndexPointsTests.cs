using Olevin.Api.Features.FinancialWellness.Helpers;

namespace Olevin.Tests.Features.FinancialWellness.Helpers;

public sealed class IndexPointsTests
{
    private const double Precision = 1e-9;

    [Theory]
    [InlineData(58.66, 58.7)]
    [InlineData(58.64, 58.6)]
    [InlineData(-0.26, -0.3)]
    public void Round_KeepsOneDecimal(double points, double expected) =>
        Assert.Equal(expected, IndexPoints.Round(points), Precision);

    [Fact]
    public void Delta_IsTheDifferenceOfTheRoundedIndexes() =>
        Assert.Equal(3.6, IndexPoints.Delta(58.74, 55.06), Precision);

    [Fact]
    public void Delta_IndexFell_IsNegative() =>
        Assert.Equal(-2.5, IndexPoints.Delta(52.6, 55.1), Precision);
}
