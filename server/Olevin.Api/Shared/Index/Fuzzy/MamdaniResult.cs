namespace Olevin.Api.Shared.Index.Fuzzy;

/// <summary>
/// The output of a Mamdani system together with the intermediate values that explain it.
/// </summary>
/// <param name="Output">The crisp output (centroid).</param>
/// <param name="InputDegrees">The degrees of membership of every input in every term.</param>
/// <param name="RuleStrengths">The firing strength α of every rule, in the order of the rules.</param>
/// <param name="TermLevels">The level β at which every output term is clipped.</param>
public sealed record MamdaniResult(
    double Output,
    IReadOnlyList<double[]> InputDegrees,
    IReadOnlyList<RuleStrength> RuleStrengths,
    IReadOnlyList<double> TermLevels
);
