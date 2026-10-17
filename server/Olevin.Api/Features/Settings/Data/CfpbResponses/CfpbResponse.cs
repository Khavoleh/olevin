using Olevin.Api.Features.Settings.Data.Users;

namespace Olevin.Api.Features.Settings.Data.CfpbResponses;

/// <summary>
/// CFPB Financial Well-Being Scale answers given once at onboarding, a row of <c>settings.cfpb_responses</c>.
/// </summary>
public sealed class CfpbResponse
{
    /// <summary>
    /// Response identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Owning user identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Response values of the 10 statements in order.
    /// </summary>
    public short[] Answers { get; set; } = [];

    /// <summary>
    /// Whether the respondent is 62 or older.
    /// </summary>
    public bool Is62Plus { get; set; }

    /// <summary>
    /// Financial well-being score from the self-administered conversion table.
    /// </summary>
    public short Score { get; set; }

    /// <summary>
    /// When the response was submitted.
    /// </summary>
    public DateTime SubmittedAt { get; set; }

    /// <summary>
    /// The owning user.
    /// </summary>
    public User User { get; set; } = null!;
}
