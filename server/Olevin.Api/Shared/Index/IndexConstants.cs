namespace Olevin.Api.Shared.Index;

/// <summary>
/// The parameters of the index outside the fuzzy model.
/// </summary>
public static class IndexConstants
{
    /// <summary>
    /// The version of the model parameters. Every stored result keeps it, so it changes with any parameter.
    /// </summary>
    public const string ModelVersion = "1.0";

    /// <summary>
    /// How many snapshots the average expenses in R cover at most.
    /// </summary>
    public const int AverageWindow = 3;

    /// <summary>
    /// How many snapshots CV and RG cover at most.
    /// </summary>
    public const int HistoryWindow = 6;

    /// <summary>
    /// The self-rating that does not change the index.
    /// </summary>
    public const int NeutralSelfRating = 3;

    /// <summary>
    /// The largest correction by the self-rating, in points.
    /// </summary>
    public const double MaxSelfRatingCorrection = 5;

    /// <summary>
    /// The number of actual snapshots from which the index is no longer preliminary.
    /// </summary>
    public const int FinalFromSnapshots = 3;

    /// <summary>
    /// The lowest value of the "unstable" zone.
    /// </summary>
    public const double UnstableFrom = 40;

    /// <summary>
    /// The lowest value of the "calm" zone.
    /// </summary>
    public const double CalmFrom = 70;
}
