namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.GetOpenMonths.Responses;

/// <summary>
/// The months a new snapshot may be created for.
/// </summary>
/// <param name="Months">The open months without a snapshot, the latest first; empty until the next one opens.</param>
/// <param name="OpensOn">
/// The day the next month opens for a snapshot, or <see langword="null"/> when a month is open now.
/// </param>
public sealed record GetOpenMonthsResponse(
    IReadOnlyList<OpenMonthResponse> Months,
    DateOnly? OpensOn
);
