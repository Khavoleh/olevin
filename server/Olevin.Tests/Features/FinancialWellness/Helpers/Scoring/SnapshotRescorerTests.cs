using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Data.EstimatedSnapshots;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Helpers;
using Olevin.Api.Features.FinancialWellness.Helpers.History;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Features.FinancialWellness.Helpers.Scoring;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Mappings;
using Olevin.Api.Shared;
using Olevin.Tests.TestData;

namespace Olevin.Tests.Features.FinancialWellness.Helpers.Scoring;

public sealed class SnapshotRescorerTests
{
    private static readonly DateOnly FxDate = new(2026, 9, 25);

    private static readonly Dictionary<DateOnly, ExchangeRates> Rates = new()
    {
        [FxDate] = new ExchangeRates(ControlExample.UnitsPerUsd),
    };

    private static readonly DateTime Now = new(2026, 10, 16, 12, 0, 0, DateTimeKind.Utc);

    private static EstimatedSnapshot Estimate(CalendarMonth month) =>
        new()
        {
            Year = 2026,
            Month = month,
            FxDate = FxDate,
            ExpensesCurrency = CurrencyCode.UAH,
            ExpensesAmount = 30_000,
            SavedCurrency = CurrencyCode.UAH,
            SavedAmount = 5_000,
        };

    private static Snapshot Stored(CalendarMonth month, int selfRating = 4) =>
        SnapshotMapping.ToSnapshot(
            Guid.CreateVersion7(),
            new SnapshotMonth(2026, month),
            FxDate,
            ControlExample.SeptemberAmounts(),
            selfRating
        );

    private static Dictionary<SnapshotMonth, SnapshotScore> Score(params Snapshot[] snapshots) =>
        SnapshotRescorer.Score(
            [Estimate(CalendarMonth.June), Estimate(CalendarMonth.May)],
            snapshots,
            Rates,
            CurrencyCode.UAH
        );

    private static (SnapshotMonth, IndicatorVector, double, double) Observed(SnapshotScore score) =>
        (score.Month, score.Inputs.Indicators, score.Index.Linear, score.Index.Final);

    [Fact]
    public void Score_OrdersTheHistoryByMonth()
    {
        Snapshot july = Stored(CalendarMonth.July);
        Snapshot august = Stored(CalendarMonth.August);
        Snapshot september = Stored(CalendarMonth.September);
        SnapshotFigures[] ordered =
        [
            SnapshotHistoryMapping.ToFigures(
                Estimate(CalendarMonth.May),
                Rates[FxDate],
                CurrencyCode.UAH
            ),
            SnapshotHistoryMapping.ToFigures(
                Estimate(CalendarMonth.June),
                Rates[FxDate],
                CurrencyCode.UAH
            ),
            SnapshotHistoryMapping.ToFigures(july, Rates[FxDate], CurrencyCode.UAH),
            SnapshotHistoryMapping.ToFigures(august, Rates[FxDate], CurrencyCode.UAH),
            SnapshotHistoryMapping.ToFigures(september, Rates[FxDate], CurrencyCode.UAH),
        ];

        Dictionary<SnapshotMonth, SnapshotScore> scores = Score(september, july, august);

        Assert.Equal(
            SnapshotScorer.Score(ordered).Select(Observed),
            scores.Values.OrderBy(score => score.Month.Month).Select(Observed)
        );
    }

    [Fact]
    public void Score_OfAChangedMonth_ChangesTheLaterOnesOnly()
    {
        Dictionary<SnapshotMonth, SnapshotScore> before = Score(
            Stored(CalendarMonth.July),
            Stored(CalendarMonth.August),
            Stored(CalendarMonth.September)
        );
        Snapshot august = Stored(CalendarMonth.August);
        august.Amounts = august.Amounts with { Saved = ControlExample.Uah(0) };

        Dictionary<SnapshotMonth, SnapshotScore> after = Score(
            Stored(CalendarMonth.July),
            august,
            Stored(CalendarMonth.September)
        );

        SnapshotMonth july = new(2026, CalendarMonth.July);
        SnapshotMonth september = new(2026, CalendarMonth.September);
        Assert.Equal(Observed(before[july]), Observed(after[july]));
        Assert.NotEqual(
            before[september].Inputs.Indicators.SavingsRegularity,
            after[september].Inputs.Indicators.SavingsRegularity
        );
    }

    [Fact]
    public void ApplyFrom_ScoresTheMonthAndTheLaterOnes()
    {
        Snapshot july = Stored(CalendarMonth.July);
        Snapshot august = Stored(CalendarMonth.August);
        Snapshot september = Stored(CalendarMonth.September);
        Dictionary<SnapshotMonth, SnapshotScore> scores = Score(july, august, september);

        SnapshotRescorer.ApplyFrom(
            [july, august, september],
            new SnapshotMonth(2026, CalendarMonth.August),
            scores,
            Now
        );

        Assert.Equal(0, july.FinalIndex);
        Assert.Equal(default, july.UpdatedAt);
        Assert.Equal(
            scores[new SnapshotMonth(2026, CalendarMonth.August)].Index.Final,
            august.FinalIndex
        );
        Assert.Equal(Now, august.UpdatedAt);
        Assert.Equal(
            scores[new SnapshotMonth(2026, CalendarMonth.September)].Index.Final,
            september.FinalIndex
        );
        Assert.Equal(Now, september.UpdatedAt);
    }

    [Fact]
    public void ApplyFrom_TheLatestMonth_ScoresOnlyIt()
    {
        Snapshot august = Stored(CalendarMonth.August);
        Snapshot september = Stored(CalendarMonth.September);
        Dictionary<SnapshotMonth, SnapshotScore> scores = Score(august, september);

        SnapshotRescorer.ApplyFrom(
            [august, september],
            new SnapshotMonth(2026, CalendarMonth.September),
            scores,
            Now
        );

        Assert.Equal(default, august.UpdatedAt);
        Assert.Equal(Now, september.UpdatedAt);
    }

    [Fact]
    public void ApplyFrom_ADeletedMonth_ScoresTheLaterOnesWithoutIt()
    {
        Snapshot july = Stored(CalendarMonth.July);
        Snapshot september = Stored(CalendarMonth.September);
        SnapshotMonth septemberMonth = new(2026, CalendarMonth.September);
        Dictionary<SnapshotMonth, SnapshotScore> withAugust = Score(
            Stored(CalendarMonth.July),
            Stored(CalendarMonth.August),
            Stored(CalendarMonth.September)
        );
        Dictionary<SnapshotMonth, SnapshotScore> scores = Score(july, september);

        SnapshotRescorer.ApplyFrom(
            [july, september],
            new SnapshotMonth(2026, CalendarMonth.August),
            scores,
            Now
        );

        Assert.Equal(default, july.UpdatedAt);
        Assert.Equal(Now, september.UpdatedAt);
        Assert.Equal(scores[septemberMonth].Index.Final, september.FinalIndex);
        Assert.NotEqual(Observed(withAugust[septemberMonth]), Observed(scores[septemberMonth]));
    }
}
