using Olevin.Api.Features.Settings.Slices.CfpbResponses.Handlers;

namespace Olevin.Tests.Features.Settings.Slices.CfpbResponses.Handlers;

/// <summary>
/// The conversion of the CFPB total response value to the score.
/// </summary>
public sealed class CfpbScoreTableHelperTests
{
    [Theory]
    [InlineData(0, false, 14)]
    [InlineData(12, false, 40)]
    [InlineData(40, false, 86)]
    [InlineData(0, true, 14)]
    [InlineData(36, true, 79)]
    [InlineData(40, true, 95)]
    public void ToScore_ReturnsWorksheetValue(int total, bool is62Plus, short expected) =>
        Assert.Equal(expected, CfpbScoreTableHelper.ToScore(total, is62Plus));

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ToScore_GrowsWithTotal(bool is62Plus) =>
        Assert.All(
            Enumerable.Range(1, CfpbScoreTableHelper.MaxTotal),
            total =>
                Assert.True(
                    CfpbScoreTableHelper.ToScore(total, is62Plus)
                        > CfpbScoreTableHelper.ToScore(total - 1, is62Plus)
                )
        );
}
