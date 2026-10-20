using System.Linq.Expressions;
using FluentValidation;

namespace Olevin.Api.Features.Settings.Slices.Onboarding.Handlers.SubmitCfpbResponse;

/// <summary>
/// Validates the answers to the CFPB Financial Well-Being Scale: every statement is answered with one of its options.
/// </summary>
public sealed class SubmitCfpbResponseValidator : AbstractValidator<SubmitCfpbResponseRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubmitCfpbResponseValidator"/> class.
    /// </summary>
    public SubmitCfpbResponseValidator()
    {
        Answer(request => request.CouldHandleMajorExpense, 1);
        Answer(request => request.SecuringFinancialFuture, 2);
        Answer(request => request.NeverHaveThingsWanted, 3);
        Answer(request => request.EnjoyLifeManagingMoney, 4);
        Answer(request => request.JustGettingBy, 5);
        Answer(request => request.MoneyWontLast, 6);
        Answer(request => request.GiftStrainsFinances, 7);
        Answer(request => request.MoneyLeftOver, 8);
        Answer(request => request.BehindWithFinances, 9);
        Answer(request => request.FinancesControlLife, 10);

        RuleFor(request => request.Is62Plus).NotNull().WithMessage("Age group is required.");
    }

    private void Answer<TAnswer>(
        Expression<Func<SubmitCfpbResponseRequest, TAnswer>> answer,
        int statement
    )
        where TAnswer : struct, Enum =>
        RuleFor(answer)
            .NotNull()
            .WithMessage($"Statement {statement} must be answered.")
            .IsInEnum()
            .WithMessage($"Answer to statement {statement} is not one of its options.");
}
