using Olevin.Api.Features.Advice;
using Olevin.Api.Shared.Index.Indicators;
using Olevin.Tests.TestData;

namespace Olevin.Tests.Features.Advice;

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
