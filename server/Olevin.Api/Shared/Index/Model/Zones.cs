namespace Olevin.Api.Shared.Index.Model;

/// <summary>
/// Places index values into zones.
/// </summary>
public static class Zones
{
    /// <summary>
    /// Gets the zone of an index value.
    /// </summary>
    /// <param name="value">The index from 0 to 100.</param>
    /// <returns>The zone.</returns>
    public static Zone Of(double value)
    {
        return value switch
        {
            < IndexConstants.UnstableFrom => Zone.Stress,
            < IndexConstants.CalmFrom => Zone.Unstable,
            _ => Zone.Calm,
        };
    }
}
