using System.Text.Json.Serialization;
using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;

namespace Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Responses;

/// <summary>
/// One advice; <c>kind</c> names the indicator it improves.
/// </summary>
/// <param name="Gain">The rise of the final index in points, rounded to one decimal.</param>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(SavingsRateAdviceResponse), nameof(Indicator.SavingsRate))]
[JsonDerivedType(typeof(ReserveAdviceResponse), nameof(Indicator.Reserve))]
[JsonDerivedType(typeof(DebtToIncomeAdviceResponse), nameof(Indicator.DebtToIncome))]
[JsonDerivedType(typeof(ExpenseVariationAdviceResponse), nameof(Indicator.ExpenseVariation))]
[JsonDerivedType(typeof(SavingsRegularityAdviceResponse), nameof(Indicator.SavingsRegularity))]
public abstract record AdviceResponse(double Gain);
