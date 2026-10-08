namespace Olevin.Api.Shared.Index.Fuzzy;

/// <summary>
/// A rule "if x₁ is A₁ and … and xₙ is Aₙ then y is C".
/// </summary>
/// <param name="Number">The rule number used in the documentation and tests.</param>
/// <param name="Conditions">The term index of every input, in the order of the inputs of the system.</param>
/// <param name="Conclusion">The term index of the output.</param>
public sealed record FuzzyRule(int Number, IReadOnlyList<int> Conditions, int Conclusion);
