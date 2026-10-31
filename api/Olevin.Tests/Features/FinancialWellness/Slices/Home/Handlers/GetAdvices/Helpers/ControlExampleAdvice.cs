using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Helpers;
using Olevin.Api.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Models;
using Olevin.Tests.TestData;

namespace Olevin.Tests.Features.FinancialWellness.Slices.Home.Handlers.GetAdvices.Helpers;

internal static class ControlExampleAdvice
{
    public static CalculatedInputs SeptemberInputs()
    {
        MonthFigures[] months = ControlExample.Months();

        return InputsCalculator.Calculate(months, months.Length - 1);
    }

    public static IReadOnlyList<AdviceItem> September() =>
        AdviceBuilder.Build(ControlExample.Months()[^1], SeptemberInputs());
}
