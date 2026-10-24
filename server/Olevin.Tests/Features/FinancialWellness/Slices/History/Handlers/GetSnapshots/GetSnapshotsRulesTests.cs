using Olevin.Api.Features.FinancialWellness.Slices.History.Handlers.GetSnapshots;

namespace Olevin.Tests.Features.FinancialWellness.Slices.History.Handlers.GetSnapshots;

public sealed class GetSnapshotsRulesTests
{
    private const double Precision = 1e-9;

    [Fact]
    public void ChangeAt_IsTheDifferenceOfTheShownIndexes()
    {
        double[] newestFirst = [58.66, 55.14, 50.2];

        Assert.Equal(3.6, GetSnapshotsRules.ChangeAt(newestFirst, 0)!.Value, Precision);
        Assert.Equal(4.9, GetSnapshotsRules.ChangeAt(newestFirst, 1)!.Value, Precision);
    }

    [Fact]
    public void ChangeAt_OfTheFirstSnapshot_IsNull() =>
        Assert.Null(GetSnapshotsRules.ChangeAt([58.66, 55.14], 1));

    [Fact]
    public void RoundMoney_KeepsTwoDecimals() =>
        Assert.Equal(1234.57, GetSnapshotsRules.RoundMoney(1234.5678), Precision);
}
