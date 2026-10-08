namespace Olevin.Api.Shared.Index.Fuzzy;

/// <summary>
/// A trapezoidal membership function <c>μ(x; a, b, c, d)</c>. A triangle is the case <c>b = c</c>.
/// </summary>
/// <remarks>
/// For an edge term with <c>a = b</c> (or <c>c = d</c>) the matching side is treated as 1, so the term stays fully
/// true up to the edge of the range instead of dividing by zero.
/// </remarks>
/// <param name="A">The left point where the degree starts to rise from 0.</param>
/// <param name="B">The left point where the degree reaches 1.</param>
/// <param name="C">The right point where the degree starts to fall from 1.</param>
/// <param name="D">The right point where the degree falls back to 0.</param>
public readonly record struct MembershipFunction(double A, double B, double C, double D)
{
    /// <summary>
    /// Creates a triangular function with the peak at <paramref name="b"/>.
    /// </summary>
    /// <param name="a">The left point where the degree is 0.</param>
    /// <param name="b">The peak where the degree is 1.</param>
    /// <param name="c">The right point where the degree is 0.</param>
    /// <returns>The triangular function.</returns>
    public static MembershipFunction Triangle(double a, double b, double c) => new(a, b, b, c);

    /// <summary>
    /// Creates a trapezoidal function.
    /// </summary>
    /// <param name="a">The left point where the degree is 0.</param>
    /// <param name="b">The left point where the degree reaches 1.</param>
    /// <param name="c">The right point where the degree starts to fall.</param>
    /// <param name="d">The right point where the degree is 0.</param>
    /// <returns>The trapezoidal function.</returns>
    public static MembershipFunction Trapezoid(double a, double b, double c, double d) =>
        new(a, b, c, d);

    /// <summary>
    /// Calculates the degree of membership of a value.
    /// </summary>
    /// <param name="x">The value.</param>
    /// <returns>The degree from 0 to 1.</returns>
    public double Evaluate(double x)
    {
        double rising = B > A ? (x - A) / (B - A) : 1;
        double falling = D > C ? (D - x) / (D - C) : 1;

        return Math.Max(0, Math.Min(Math.Min(rising, 1), falling));
    }
}
