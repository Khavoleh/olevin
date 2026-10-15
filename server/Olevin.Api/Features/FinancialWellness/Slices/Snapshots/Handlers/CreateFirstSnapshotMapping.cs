using Olevin.Api.Features.FinancialWellness.Data.EstimatedSnapshots;
using Olevin.Api.Features.FinancialWellness.Data.Snapshots;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers.Requests;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Mappings;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Models;
using Olevin.Api.Shared;

namespace Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Handlers;

/// <summary>
/// Maps the estimates of the onboarding to the stored rows and to the figures of the core.
/// </summary>
public static class CreateFirstSnapshotMapping
{
    /// <summary>
    /// Creates the stored amounts of the first snapshot.
    /// </summary>
    /// <param name="request">The validated request.</param>
    /// <returns>The amounts as they are stored.</returns>
    public static SnapshotAmounts ToAmounts(CreateFirstSnapshotRequest request) =>
        SnapshotAmountsMapping.ToAmounts(
            request.Income,
            request.Expenses,
            request.Saved,
            request.Reserves,
            request.DebtPayments
        );

    /// <summary>
    /// Lists the currencies an estimate needs rates for.
    /// </summary>
    /// <param name="estimate">The validated estimate.</param>
    /// <param name="baseCurrency">The base currency of the user.</param>
    /// <returns>The currencies of both amounts and the base currency.</returns>
    public static IEnumerable<CurrencyCode> Currencies(
        EstimateRequest estimate,
        CurrencyCode baseCurrency
    ) => [estimate.ExpensesTotal.Currency, estimate.Saved.Currency, baseCurrency];

    /// <summary>
    /// Converts an estimate to the figures of the core in the base currency.
    /// </summary>
    /// <param name="estimate">The validated estimate.</param>
    /// <param name="rates">The rates of the estimated month.</param>
    /// <param name="baseCurrency">The base currency of the user.</param>
    /// <returns>The figures of the estimated month.</returns>
    public static MonthFigures ToFigures(
        EstimateRequest estimate,
        SnapshotRates rates,
        CurrencyCode baseCurrency
    )
    {
        double ToBase(Money money) => (double)rates.Rates.Convert(money, baseCurrency);

        return MonthFigures.Estimated(ToBase(estimate.ExpensesTotal), ToBase(estimate.Saved));
    }

    /// <summary>
    /// Creates the stored estimate.
    /// </summary>
    /// <param name="userId">The owning user identifier.</param>
    /// <param name="month">The estimated month.</param>
    /// <param name="estimate">The validated estimate.</param>
    /// <param name="rates">The rates of the estimated month.</param>
    /// <returns>The estimated snapshot with a new identifier.</returns>
    public static EstimatedSnapshot ToEstimatedSnapshot(
        Guid userId,
        SnapshotMonth month,
        EstimateRequest estimate,
        SnapshotRates rates
    )
    {
        return new EstimatedSnapshot
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Month = month.Month,
            Year = month.Year,
            FxDate = rates.FxDate,
            ExpensesCurrency = estimate.ExpensesTotal.Currency,
            ExpensesAmount = estimate.ExpensesTotal.Amount,
            SavedCurrency = estimate.Saved.Currency,
            SavedAmount = estimate.Saved.Amount,
        };
    }
}
