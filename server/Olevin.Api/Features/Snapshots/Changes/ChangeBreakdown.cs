namespace Olevin.Api.Features.Snapshots.Changes;

/// <summary>
/// The change of I_fuzzy between two snapshots, split into contributions.
/// </summary>
/// <param name="Previous">I_fuzzy of the previous actual snapshot.</param>
/// <param name="Current">I_fuzzy of this snapshot.</param>
/// <param name="Contributions">The contribution C_k of every indicator.</param>
/// <param name="JointEffect">The joint effect C_joint, so that the contributions add up to the change.</param>
public sealed record ChangeBreakdown(
    double Previous,
    double Current,
    IReadOnlyList<IndicatorContribution> Contributions,
    double JointEffect
)
{
    /// <summary>
    /// Gets the actual change of I_fuzzy.
    /// </summary>
    public double Total => Current - Previous;
}
