namespace Olevin.Api.Features.Settings.Slices.Onboarding.Handlers.GetOnboardingStatus;

/// <summary>
/// The part of the onboarding the signed-in user has to go through next.
/// </summary>
public enum OnboardingStatus
{
    /// <summary>
    /// The user does not exist yet.
    /// </summary>
    NotExist,

    /// <summary>
    /// The user has no settings.
    /// </summary>
    Settings,

    /// <summary>
    /// The user has no first snapshot with the estimates of the 2 months before it.
    /// </summary>
    Onboarding,

    /// <summary>
    /// The user has consented to research but has not answered the CFPB questionnaire.
    /// </summary>
    CfpbResponse,

    /// <summary>
    /// The user has gone through the whole onboarding.
    /// </summary>
    Completed,
}
