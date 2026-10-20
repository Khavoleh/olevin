using Olevin.Api.Features.Settings.Slices.Onboarding.Handlers.SubmitCfpbResponse;
using Olevin.Api.Features.Settings.Slices.Onboarding.Handlers.SubmitCfpbResponse.Enums;

namespace Olevin.Tests.Features.Settings.Slices.Onboarding.Handlers.SubmitCfpbResponse;

public sealed class SubmitCfpbResponseMappingTests
{
    private static SubmitCfpbResponseRequest Best(bool is62Plus) =>
        new(
            CfpbDescription.Completely,
            CfpbDescription.Completely,
            CfpbDescription.NotAtAll,
            CfpbDescription.Completely,
            CfpbDescription.NotAtAll,
            CfpbDescription.NotAtAll,
            CfpbFrequency.Never,
            CfpbFrequency.Always,
            CfpbFrequency.Never,
            CfpbFrequency.Never,
            is62Plus
        );

    private static SubmitCfpbResponseRequest Worst() =>
        new(
            CfpbDescription.NotAtAll,
            CfpbDescription.NotAtAll,
            CfpbDescription.Completely,
            CfpbDescription.NotAtAll,
            CfpbDescription.Completely,
            CfpbDescription.Completely,
            CfpbFrequency.Always,
            CfpbFrequency.Never,
            CfpbFrequency.Always,
            CfpbFrequency.Always,
            false
        );

    [Fact]
    public void MostFavourableOptions_AreCodedAsFour() =>
        Assert.All(
            SubmitCfpbResponseMapping.ToAnswers(Best(false)),
            answer => Assert.Equal(4, answer)
        );

    [Fact]
    public void LeastFavourableOptions_AreCodedAsZero() =>
        Assert.All(SubmitCfpbResponseMapping.ToAnswers(Worst()), answer => Assert.Equal(0, answer));

    [Fact]
    public void MiddleOption_IsCodedAsTwo() =>
        Assert.Equal(
            2,
            SubmitCfpbResponseMapping.ToAnswers(
                Worst() with
                {
                    NeverHaveThingsWanted = CfpbDescription.Somewhat,
                }
            )[2]
        );

    [Theory]
    [InlineData(false, 86)]
    [InlineData(true, 95)]
    public void BestAnswers_ScoreTheTopOfTheColumn(bool is62Plus, short expected) =>
        Assert.Equal(expected, Score(Best(is62Plus)));

    [Fact]
    public void GuideExample_TotalOfTwelve_ScoresForty()
    {
        SubmitCfpbResponseRequest request = Worst() with
        {
            CouldHandleMajorExpense = CfpbDescription.Completely,
            SecuringFinancialFuture = CfpbDescription.Completely,
            NeverHaveThingsWanted = CfpbDescription.NotAtAll,
        };

        Assert.Equal(12, SubmitCfpbResponseMapping.ToAnswers(request).Sum(answer => answer));
        Assert.Equal(40, Score(request));
    }

    private static short Score(SubmitCfpbResponseRequest request) =>
        CfpbScoreTableHelper.ToScore(
            SubmitCfpbResponseMapping.ToAnswers(request).Sum(answer => answer),
            request.Is62Plus
        );
}
