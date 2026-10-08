namespace Olevin.Api.Shared.Index.Model;

/// <summary>
/// The indexes of one snapshot.
/// </summary>
/// <param name="Fuzzy">The results of the fuzzy subsystems.</param>
/// <param name="Linear">The linear index I_lin.</param>
/// <param name="Final">The final index I: I_fuzzy corrected by the self-rating.</param>
/// <param name="IsPreliminary">Whether there are fewer than 3 actual snapshots yet.</param>
public sealed record IndexResult(
    FuzzyEvaluation Fuzzy,
    double Linear,
    double Final,
    bool IsPreliminary
)
{
    /// <summary>
    /// Gets the "cash flow" subsystem from 0 to 100.
    /// </summary>
    public double CashFlow => Fuzzy.CashFlow.Output;

    /// <summary>
    /// Gets the "financial buffer" subsystem from 0 to 100.
    /// </summary>
    public double Buffer => Fuzzy.Buffer.Output;

    /// <summary>
    /// Gets the fuzzy index I_fuzzy.
    /// </summary>
    public double FuzzyIndex => Fuzzy.Value;

    /// <summary>
    /// Gets the zone of the final index.
    /// </summary>
    public Zone Zone => Zones.Of(Final);
}
