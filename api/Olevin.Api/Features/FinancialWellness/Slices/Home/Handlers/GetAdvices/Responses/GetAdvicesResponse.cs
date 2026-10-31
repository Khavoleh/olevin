using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Enums;
using Olevin.Api.Shared;

namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Responses;

/// <summary>
/// The advice that raises the latest index of the user most.
/// </summary>
/// <param name="Status">Whether there is advice, and why not when <paramref name="Advices"/> is empty.</param>
/// <param name="Currency">The base currency of every amount.</param>
/// <param name="Advices">At most three advice, the largest gain first.</param>
public sealed record GetAdvicesResponse(
    AdviceStatus Status,
    CurrencyCode Currency,
    IReadOnlyList<AdviceResponse> Advices
);
