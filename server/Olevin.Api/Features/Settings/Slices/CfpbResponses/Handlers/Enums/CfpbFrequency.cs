namespace Olevin.Api.Features.Settings.Slices.CfpbResponses.Handlers.Enums;

/// <summary>
/// How often a statement of the CFPB scale applies to the respondent (statements 7-10).
/// </summary>
public enum CfpbFrequency
{
    /// <summary>
    /// Always applies.
    /// </summary>
    Always,

    /// <summary>
    /// Often applies.
    /// </summary>
    Often,

    /// <summary>
    /// Sometimes applies.
    /// </summary>
    Sometimes,

    /// <summary>
    /// Rarely applies.
    /// </summary>
    Rarely,

    /// <summary>
    /// Never applies.
    /// </summary>
    Never,
}
