using FluentValidation;
using Olevin.Api.Shared.Index.Fuzzy;

namespace Olevin.Tests.Shared.Index.Fuzzy;

public sealed class LinguisticVariableTests
{
    private static readonly LinguisticTerm Term = new("any", MembershipFunction.Triangle(0, 1, 2));

    [Fact]
    public void Constructor_RejectsAnEmptyRange()
    {
        ValidationException exception = Assert.Throws<ValidationException>(() =>
            new LinguisticVariable("x", 1, 1, [Term])
        );

        Assert.Equal(nameof(LinguisticVariable.Min), Assert.Single(exception.Errors).PropertyName);
    }

    [Fact]
    public void Constructor_RejectsAVariableWithoutTerms()
    {
        ValidationException exception = Assert.Throws<ValidationException>(() =>
            new LinguisticVariable("x", 0, 2, [])
        );

        Assert.Equal(
            nameof(LinguisticVariable.Terms),
            Assert.Single(exception.Errors).PropertyName
        );
    }

    [Fact]
    public void Fuzzify_LimitsTheValueToTheRangeFirst()
    {
        LinguisticVariable variable = new("x", 0, 2, [Term]);

        Assert.Equal(variable.Fuzzify(2), variable.Fuzzify(5));
    }
}
