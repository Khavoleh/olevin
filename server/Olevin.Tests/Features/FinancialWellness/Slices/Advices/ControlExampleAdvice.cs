using Olevin.Api.Features.FinancialWellness.Helpers.Index.Indicators;
using Olevin.Api.Features.FinancialWellness.Slices.Advices;
using Olevin.Tests.TestData;

namespace Olevin.Tests.Features.FinancialWellness.Slices.Advices;

internal static class ControlExampleAdvice
{
    public static IReadOnlyList<AdviceItem> September()
    {
        MonthFigures[] months = ControlExample.Months();
        int september = months.Length - 1;

        return AdviceBuilder.Build(
            months[september],
            InputsCalculator.Calculate(months, september)
        );
    }
}
