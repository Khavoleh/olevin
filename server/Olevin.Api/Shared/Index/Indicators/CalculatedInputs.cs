namespace Olevin.Api.Shared.Index.Indicators;

/// <summary>
/// The model inputs of a snapshot together with the values they are calculated from.
/// </summary>
/// <param name="Indicators">The inputs limited to their ranges; the model and advice use them.</param>
/// <param name="Raw">The inputs before limiting, for display.</param>
/// <param name="AvailableSnapshots">The number of snapshots up to this one, estimates included (n_t).</param>
/// <param name="AverageExpenses">The average expenses over the last min(3, n_t) snapshots (E_avg).</param>
/// <param name="MeanExpenses">The mean expenses over the last min(6, n_t) snapshots (μ).</param>
/// <param name="ExpenseDeviation">The standard deviation of those expenses, divided by m (σ).</param>
/// <param name="SavingMonths">The number of those snapshots with something saved (m₊).</param>
/// <param name="HistoryMonths">The number of snapshots CV and RG cover (m).</param>
public sealed record CalculatedInputs(
    IndicatorVector Indicators,
    IndicatorVector Raw,
    int AvailableSnapshots,
    double AverageExpenses,
    double MeanExpenses,
    double ExpenseDeviation,
    int SavingMonths,
    int HistoryMonths
);
