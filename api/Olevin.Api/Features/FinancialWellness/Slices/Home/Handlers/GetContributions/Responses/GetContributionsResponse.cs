namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetContributions.Responses;

/// <summary>
/// How much each factor changed the index since the previous snapshot.
/// </summary>
/// <param name="SavingsRate">The savings rate SR.</param>
/// <param name="Reserve">The reserve R in months.</param>
/// <param name="DebtToIncome">The debt-to-income DTI.</param>
/// <param name="ExpenseVariation">The expense variation CV.</param>
/// <param name="SavingsRegularity">The savings regularity RG.</param>
/// <param name="SelfRating">The self-rating.</param>
public sealed record GetContributionsResponse(
    IndicatorContributionResponse SavingsRate,
    IndicatorContributionResponse Reserve,
    IndicatorContributionResponse DebtToIncome,
    IndicatorContributionResponse ExpenseVariation,
    SavingsRegularityContributionResponse SavingsRegularity,
    SelfRatingContributionResponse SelfRating
);
