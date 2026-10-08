using FluentValidation;

namespace Olevin.Api.Shared.Index.Fuzzy;

/// <summary>
/// A variable described by words: a range of values and the terms that cover it.
/// </summary>
public sealed class LinguisticVariable
{
    private static readonly LinguisticVariableValidator Validator = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="LinguisticVariable"/> class.
    /// </summary>
    /// <param name="name">The variable name.</param>
    /// <param name="min">The lowest value of the range.</param>
    /// <param name="max">The highest value of the range.</param>
    /// <param name="terms">The terms in their order; the position of a term is its index in the rules.</param>
    /// <exception cref="ValidationException">The variable is not valid.</exception>
    public LinguisticVariable(
        string name,
        double min,
        double max,
        IReadOnlyList<LinguisticTerm> terms
    )
    {
        Name = name;
        Min = min;
        Max = max;
        Terms = terms;

        Validator.ValidateAndThrow(this);
    }

    /// <summary>
    /// Gets the variable name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the lowest value of the range.
    /// </summary>
    public double Min { get; }

    /// <summary>
    /// Gets the highest value of the range.
    /// </summary>
    public double Max { get; }

    /// <summary>
    /// Gets the terms in their order.
    /// </summary>
    public IReadOnlyList<LinguisticTerm> Terms { get; }

    /// <summary>
    /// Limits a value to the range of the variable.
    /// </summary>
    /// <param name="x">The value.</param>
    /// <returns>The value within <see cref="Min"/> and <see cref="Max"/>.</returns>
    public double Clamp(double x) => Math.Clamp(x, Min, Max);

    /// <summary>
    /// Calculates the degree of membership of a value in every term.
    /// </summary>
    /// <param name="x">The value; it is limited to the range first.</param>
    /// <returns>The degrees in the order of <see cref="Terms"/>.</returns>
    public double[] Fuzzify(double x)
    {
        double clamped = Clamp(x);
        double[] degrees = new double[Terms.Count];

        for (int i = 0; i < degrees.Length; i++)
            degrees[i] = Terms[i].Function.Evaluate(clamped);

        return degrees;
    }
}

/// <summary>
/// Validates a linguistic variable: a named, non-empty range covered by at least one term.
/// </summary>
public sealed class LinguisticVariableValidator : AbstractValidator<LinguisticVariable>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LinguisticVariableValidator"/> class.
    /// </summary>
    public LinguisticVariableValidator()
    {
        RuleFor(variable => variable.Name).NotEmpty();
        RuleFor(variable => variable.Min).LessThan(variable => variable.Max);
        RuleFor(variable => variable.Terms).NotEmpty();
    }
}
