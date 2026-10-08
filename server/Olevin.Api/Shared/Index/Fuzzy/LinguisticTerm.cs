namespace Olevin.Api.Shared.Index.Fuzzy;

/// <summary>
/// A named fuzzy set of a linguistic variable, such as "low" or "high".
/// </summary>
/// <param name="Name">The term name.</param>
/// <param name="Function">The membership function of the term.</param>
public sealed record LinguisticTerm(string Name, MembershipFunction Function);
