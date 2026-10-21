using Olevin.Api.Features.FinancialWellness.Data;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Model;

namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.Responses;

/// <summary>
/// The latest index of the user.
/// </summary>
/// <param name="Year">The year of the latest snapshot.</param>
/// <param name="Month">The month of the latest snapshot.</param>
/// <param name="Value">The final index, rounded to one decimal.</param>
/// <param name="Zone">The zone of <paramref name="Value"/>.</param>
/// <param name="Change">The change since the previous snapshot, or <see langword="null"/> for the first one.</param>
/// <param name="NextSnapshotDate">The day the next snapshot is due.</param>
public sealed record GetIndexResponse(
    short Year,
    CalendarMonth Month,
    double Value,
    Zone Zone,
    IndexChangeResponse? Change,
    DateOnly NextSnapshotDate
);
