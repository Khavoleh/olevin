namespace Olevin.Api.Features.Settings.Slices.Onboarding.Handlers.SubmitCfpbResponse.Enums;

/// <summary>
/// How well a statement of the CFPB scale describes the respondent (statements 1-6).
/// </summary>
public enum CfpbDescription
{
    /// <summary>
    /// Describes me completely.
    /// </summary>
    Completely,

    /// <summary>
    /// Describes me very well.
    /// </summary>
    VeryWell,

    /// <summary>
    /// Describes me somewhat.
    /// </summary>
    Somewhat,

    /// <summary>
    /// Describes me very little.
    /// </summary>
    VeryLittle,

    /// <summary>
    /// Does not describe me at all.
    /// </summary>
    NotAtAll,
}
