namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetContributions.Responses;

/// <summary>
/// The contribution of the savings regularity RG.
/// </summary>
/// <param name="Points">The change of the index in points, rounded to one decimal, negative when it fell.</param>
/// <param name="FromSavingMonths">The months with savings in the window of the previous snapshot.</param>
/// <param name="FromMonths">The months in the window of the previous snapshot.</param>
/// <param name="ToSavingMonths">The months with savings in the window of the latest snapshot.</param>
/// <param name="ToMonths">The months in the window of the latest snapshot.</param>
public sealed record SavingsRegularityContributionResponse(
    double Points,
    int FromSavingMonths,
    int FromMonths,
    int ToSavingMonths,
    int ToMonths
);
