namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetContributions.Responses;

/// <summary>
/// The contribution of SR, R, DTI or CV.
/// </summary>
/// <param name="Points">The change of the index in points, rounded to one decimal, negative when it fell.</param>
/// <param name="From">The indicator in the previous snapshot before limiting; <see langword="null"/> where it means nothing.</param>
/// <param name="To">The indicator in the latest snapshot before limiting; <see langword="null"/> where it means nothing.</param>
public sealed record IndicatorContributionResponse(double Points, double? From, double? To);
