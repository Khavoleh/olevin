namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Enums;

/// <summary>
/// Whether there is advice for the user, and why not when there is none.
/// </summary>
public enum AdviceStatus
{
    /// <summary>
    /// At least one advice raises the index.
    /// </summary>
    Shown,

    /// <summary>
    /// The final index is already 100.
    /// </summary>
    AtMaximum,

    /// <summary>
    /// Every indicator is already in its best term, but a low self-rating keeps the final index below 100.
    /// </summary>
    AllBest,

    /// <summary>
    /// No step the person can make raises the index, for example a deficit without flexible expenses.
    /// </summary>
    NoQuickWin,
}
