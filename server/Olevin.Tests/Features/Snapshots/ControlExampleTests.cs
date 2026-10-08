using Olevin.Api.Features.Snapshots;
using Olevin.Api.Shared.Index.Indicators;
using Olevin.Api.Shared.Index.Model;
using Olevin.Tests.TestData;

namespace Olevin.Tests.Features.Snapshots;

/// <summary>
/// The control example from money amounts: inputs and indexes of September.
/// </summary>
public sealed class ControlExampleTests
{
    private const double Precision = 0.1;

    private static readonly SnapshotScore September = SnapshotScorer.Score(ControlExample.Months())[
        ^1
    ]!;

    [Fact]
    public void Inputs_MatchTheExpectedIntermediateValues()
    {
        CalculatedInputs inputs = September.Inputs;

        Assert.Equal(36_000, inputs.AverageExpenses, 1e-6);
        Assert.Equal(35_000, inputs.MeanExpenses, 1e-6);
        Assert.Equal(7_000, inputs.ExpenseDeviation, 1e-6);
        Assert.Equal(3, inputs.SavingMonths);
        Assert.Equal(6, inputs.HistoryMonths);

        Assert.Equal(0.10, inputs.Indicators.SavingsRate, 1e-9);
        Assert.Equal(3.5, inputs.Indicators.Reserve, 1e-9);
        Assert.Equal(0.30, inputs.Indicators.DebtToIncome, 1e-9);
        Assert.Equal(0.20, inputs.Indicators.ExpenseVariation, 1e-9);
        Assert.Equal(0.5, inputs.Indicators.SavingsRegularity, 1e-9);
    }

    [Fact]
    public void Indexes_MatchTheReference()
    {
        IndexResult index = September.Index;

        Assert.Equal(50.0, index.CashFlow, Precision);
        Assert.Equal(60.4, index.Buffer, Precision);
        Assert.Equal(56.2, index.FuzzyIndex, Precision);
        Assert.Equal(49.8, index.Linear, Precision);
        Assert.Equal(58.7, index.Final, Precision);
        Assert.Equal(Zone.Unstable, index.Zone);
        Assert.False(index.IsPreliminary);
        Assert.Equal("1.0", September.ModelVersion);
    }
}
