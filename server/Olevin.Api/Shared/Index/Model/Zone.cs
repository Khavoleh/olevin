namespace Olevin.Api.Shared.Index.Model;

/// <summary>
/// A zone of the index scale; the same for I, I_fuzzy and I_lin.
/// </summary>
public enum Zone
{
    /// <summary>
    /// 0–39.
    /// </summary>
    Stress,

    /// <summary>
    /// 40–69.
    /// </summary>
    Unstable,

    /// <summary>
    /// 70–100.
    /// </summary>
    Calm,
}
