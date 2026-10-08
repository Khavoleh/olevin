using FluentValidation;

namespace Olevin.Api.Shared.Index.Fuzzy;

/// <summary>
/// A Mamdani fuzzy inference system: AND = product, the level of an output term is the sum of the strengths of its
/// rules, clipping by min, aggregation by max and the centroid as a sum over the points of a grid with step 1.
/// </summary>
public sealed class MamdaniSystem
{
    private static readonly MamdaniSystemValidator Validator = new();

    /// <summary>
    /// The degree of every output term at every point of the grid, calculated once.
    /// </summary>
    private readonly double[][] GridDegrees;

    /// <summary>
    /// Initializes a new instance of the <see cref="MamdaniSystem"/> class.
    /// </summary>
    /// <param name="name">The system name.</param>
    /// <param name="inputs">The input variables.</param>
    /// <param name="output">The output variable.</param>
    /// <param name="rules">The rule base.</param>
    /// <param name="gridMin">The first point of the grid for the centroid.</param>
    /// <param name="gridMax">The last point of the grid for the centroid.</param>
    /// <remarks>
    /// The grid may be wider than the range of the output: edge terms that lie half outside the range then have
    /// their centroid exactly at the edge.
    /// </remarks>
    /// <exception cref="ValidationException">The system or one of its rules is not valid.</exception>
    public MamdaniSystem(
        string name,
        IReadOnlyList<LinguisticVariable> inputs,
        LinguisticVariable output,
        IReadOnlyList<FuzzyRule> rules,
        int gridMin,
        int gridMax
    )
    {
        Name = name;
        Inputs = inputs;
        Output = output;
        Rules = rules;
        GridMin = gridMin;
        GridMax = gridMax;

        Validator.ValidateAndThrow(this);

        GridDegrees =
        [
            .. Enumerable
                .Range(gridMin, gridMax - gridMin + 1)
                .Select(y => output.Terms.Select(term => term.Function.Evaluate(y)).ToArray()),
        ];
    }

    /// <summary>
    /// Gets the system name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the input variables.
    /// </summary>
    public IReadOnlyList<LinguisticVariable> Inputs { get; }

    /// <summary>
    /// Gets the output variable.
    /// </summary>
    public LinguisticVariable Output { get; }

    /// <summary>
    /// Gets the rule base.
    /// </summary>
    public IReadOnlyList<FuzzyRule> Rules { get; }

    /// <summary>
    /// Gets the first point of the grid for the centroid.
    /// </summary>
    public int GridMin { get; }

    /// <summary>
    /// Gets the last point of the grid for the centroid.
    /// </summary>
    public int GridMax { get; }

    /// <summary>
    /// Runs the inference for crisp inputs.
    /// </summary>
    /// <param name="values">The input values in the order of <see cref="Inputs"/>.</param>
    /// <returns>The crisp output and the intermediate values.</returns>
    public MamdaniResult Evaluate(params ReadOnlySpan<double> values)
    {
        if (values.Length != Inputs.Count)
        {
            throw new ArgumentException(
                $"{Name} expects {Inputs.Count} inputs, but got {values.Length}.",
                nameof(values)
            );
        }

        double[][] degrees = new double[Inputs.Count][];

        for (int i = 0; i < Inputs.Count; i++)
            degrees[i] = Inputs[i].Fuzzify(values[i]);

        RuleStrength[] strengths = new RuleStrength[Rules.Count];
        double[] levels = new double[Output.Terms.Count];

        for (int r = 0; r < Rules.Count; r++)
        {
            FuzzyRule rule = Rules[r];
            double strength = 1;

            for (int i = 0; i < rule.Conditions.Count; i++)
                strength *= degrees[i][rule.Conditions[i]];

            strengths[r] = new RuleStrength(rule.Number, rule.Conclusion, strength);
            levels[rule.Conclusion] += strength;
        }

        // When the edge terms are symmetric around the edges of the range, the centroid stays within the range, so
        // clamping only removes rounding noise such as −1e-15.
        return new MamdaniResult(Output.Clamp(Centroid(levels)), degrees, strengths, levels);
    }

    /// <summary>
    /// Calculates the centroid of the output terms clipped at the given levels.
    /// </summary>
    /// <param name="levels">The level β of every output term.</param>
    /// <returns>The centroid.</returns>
    /// <exception cref="InvalidOperationException">No output term is active.</exception>
    public double Centroid(IReadOnlyList<double> levels)
    {
        double numerator = 0;
        double denominator = 0;

        for (int i = 0; i < GridDegrees.Length; i++)
        {
            double[] termDegrees = GridDegrees[i];
            double degree = 0;

            for (int q = 0; q < termDegrees.Length; q++)
                degree = Math.Max(degree, Math.Min(levels[q], termDegrees[q]));

            numerator += (GridMin + i) * degree;
            denominator += degree;
        }

        return denominator > 0
            ? numerator / denominator
            : throw new InvalidOperationException(
                $"No rule of {Name} fired, so the output is undefined."
            );
    }
}

/// <summary>
/// Validates a Mamdani system: a grid of at least two points and rules that refer only to existing terms, with one
/// condition per input.
/// </summary>
public sealed class MamdaniSystemValidator : AbstractValidator<MamdaniSystem>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MamdaniSystemValidator"/> class.
    /// </summary>
    public MamdaniSystemValidator()
    {
        RuleFor(system => system.Name).NotEmpty();
        RuleFor(system => system.Inputs).NotEmpty();
        RuleFor(system => system.Rules).NotEmpty();
        RuleFor(system => system.GridMin).LessThan(system => system.GridMax);

        RuleForEach(system => system.Rules)
            .Cascade(CascadeMode.Stop)
            .Must((system, rule) => rule.Conditions.Count == system.Inputs.Count)
            .WithMessage((_, rule) => $"Rule {rule.Number} must have one condition per input.")
            .Must((system, rule) => HasConditionTerms(rule, system.Inputs))
            .WithMessage((_, rule) => $"Rule {rule.Number} refers to an unknown input term.")
            .Must((system, rule) => IsTerm(rule.Conclusion, system.Output))
            .WithMessage((_, rule) => $"Rule {rule.Number} refers to an unknown output term.");
    }

    private static bool HasConditionTerms(
        FuzzyRule rule,
        IReadOnlyList<LinguisticVariable> inputs
    ) => rule.Conditions.Select((term, i) => IsTerm(term, inputs[i])).All(isTerm => isTerm);

    private static bool IsTerm(int term, LinguisticVariable variable) =>
        term >= 0 && term < variable.Terms.Count;
}
