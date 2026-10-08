namespace Olevin.Api.Shared.Index.Fuzzy;

/// <summary>
/// The firing strength of one rule.
/// </summary>
/// <param name="Number">The rule number.</param>
/// <param name="Conclusion">The term index of the output.</param>
/// <param name="Strength">The firing strength α from 0 to 1.</param>
public sealed record RuleStrength(int Number, int Conclusion, double Strength);
