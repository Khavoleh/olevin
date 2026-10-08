using FluentValidation.TestHelper;
using Olevin.Api.Features.Snapshots.Currency;
using Olevin.Api.Features.Snapshots.Handlers.CalculateSnapshots;
using Olevin.Tests.TestData;

namespace Olevin.Tests.Features.Snapshots.Handlers.CalculateSnapshots;

public sealed class CalculateSnapshotsValidatorTests
{
    private const string September = "Snapshots[5]";

    private readonly CalculateSnapshotsValidator Validator = new();

    [Fact]
    public void ControlExample_IsValid() =>
        Validator.TestValidate(ControlExample.Request()).ShouldNotHaveAnyValidationErrors();

    [Fact]
    public void NoSnapshots_IsRejected()
    {
        CalculateSnapshotsRequest request = ControlExample.Request() with { Snapshots = [] };

        Validator.TestValidate(request).ShouldHaveValidationErrorFor(r => r.Snapshots);
    }

    [Fact]
    public void MissingRate_IsRejected()
    {
        CalculateSnapshotsRequest request = WithSeptember(s => s with { FxRates = null });

        Validator.TestValidate(request).ShouldHaveValidationErrorFor(September);
    }

    [Fact]
    public void RateForUah_IsRejected()
    {
        CalculateSnapshotsRequest request = WithSeptember(s =>
            s with
            {
                FxRates = new Dictionary<CurrencyCode, decimal>
                {
                    [CurrencyCode.USD] = 41.5m,
                    [CurrencyCode.UAH] = 1,
                },
            }
        );

        Validator.TestValidate(request).ShouldHaveValidationErrorFor($"{September}.FxRates");
    }

    [Fact]
    public void DuplicateMonths_AreRejected()
    {
        CalculateSnapshotsRequest request = ControlExample.Request();
        CalculateSnapshotsRequest duplicated = request with
        {
            Snapshots =
            [
                .. request.Snapshots,
                request.Snapshots[^1] with
                {
                    Month = new DateOnly(2026, 9, 15),
                },
            ],
        };

        Validator.TestValidate(duplicated).ShouldHaveValidationErrorFor(r => r.Snapshots);
    }

    [Fact]
    public void OnlyEstimates_AreRejected()
    {
        CalculateSnapshotsRequest request = new(
            CurrencyCode.UAH,
            [ControlExample.EstimatedRequest(new DateOnly(2026, 7, 1), 30_000, 0)]
        );

        Validator.TestValidate(request).ShouldHaveValidationErrorFor(r => r.Snapshots);
    }

    [Fact]
    public void EstimateWithIncome_IsRejected()
    {
        CalculateSnapshotsRequest request = ControlExample.Request();
        SnapshotRequest estimate = request.Snapshots[0] with
        {
            Income = new IncomeRequest(ControlExample.Uah(1_000), null),
        };

        Validator
            .TestValidate(request with { Snapshots = [estimate, .. request.Snapshots.Skip(1)] })
            .ShouldHaveValidationErrorFor("Snapshots[0].Income");
    }

    [Fact]
    public void ActualSnapshotWithoutExpenses_IsRejected()
    {
        CalculateSnapshotsRequest request = WithSeptember(s => s with { Expenses = null });

        Validator.TestValidate(request).ShouldHaveValidationErrorFor($"{September}.Expenses");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(null)]
    public void SelfRating_MustBeFrom1To5(int? selfRating)
    {
        CalculateSnapshotsRequest request = WithSeptember(s => s with { SelfRating = selfRating });

        Validator.TestValidate(request).ShouldHaveValidationErrorFor($"{September}.SelfRating");
    }

    [Fact]
    public void NegativeAmount_IsRejected()
    {
        CalculateSnapshotsRequest request = WithSeptember(s =>
            s with
            {
                Reserves = [ControlExample.Uah(-1)],
            }
        );

        Validator.TestValidate(request).ShouldHaveValidationErrorFor($"{September}.Reserves[0]");
    }

    [Fact]
    public void NegativeExpense_IsRejected()
    {
        CalculateSnapshotsRequest request = WithSeptember(s =>
            s with
            {
                Expenses = s.Expenses! with { Food = ControlExample.Uah(-1) },
            }
        );

        Validator.TestValidate(request).ShouldHaveValidationErrorFor($"{September}.Expenses.Food");
    }

    private static CalculateSnapshotsRequest WithSeptember(
        Func<SnapshotRequest, SnapshotRequest> change
    )
    {
        CalculateSnapshotsRequest request = ControlExample.Request();

        return request with
        {
            Snapshots = [.. request.Snapshots.SkipLast(1), change(request.Snapshots[^1])],
        };
    }
}
