namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetContributions.Responses;

/// <summary>
/// The contribution of the self-rating.
/// </summary>
/// <param name="Points">The change of the index in points, rounded to one decimal, negative when it fell.</param>
/// <param name="From">The self-rating of the previous snapshot from 1 to 5.</param>
/// <param name="To">The self-rating of the latest snapshot from 1 to 5.</param>
public sealed record SelfRatingContributionResponse(double Points, int From, int To);
