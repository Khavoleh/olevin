namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Helpers;

/// <summary>
/// Rounds the index and its parts the way the home page shows them.
/// </summary>
public static class IndexPoints
{
    /// <summary>
    /// Rounds an index, a subsystem or a change in points.
    /// </summary>
    /// <param name="points">The value in points.</param>
    /// <returns>The value rounded to one decimal, halves away from zero.</returns>
    public static double Round(double points) =>
        Math.Round(points, 1, MidpointRounding.AwayFromZero);

    /// <summary>
    /// Finds the change of the index, so it matches the difference of the shown numbers.
    /// </summary>
    /// <param name="latest">The index of the latest snapshot.</param>
    /// <param name="previous">The index of the snapshot before it.</param>
    /// <returns>The difference of the rounded indexes, negative when the index fell.</returns>
    public static double Delta(double latest, double previous) =>
        Round(Round(latest) - Round(previous));
}
