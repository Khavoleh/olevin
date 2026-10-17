namespace Olevin.Api.Features.Settings.Slices.CfpbResponses.Handlers;

/// <summary>
/// The conversion of the CFPB Financial Well-Being Scale total response value (0-40) to the score,
/// self-administered columns of the standard version scoring worksheet (CFPB user guide, Appendix A).
/// </summary>
public static class CfpbScoreTableHelper
{
    /// <summary>
    /// The largest total response value of the 10 statements.
    /// </summary>
    public const int MaxTotal = 40;

    private static readonly short[] Under62 =
    [
        14,
        19,
        22,
        25,
        27,
        29,
        31,
        32,
        34,
        35,
        37,
        38,
        40,
        41,
        42,
        44,
        45,
        46,
        47,
        49,
        50,
        51,
        52,
        54,
        55,
        56,
        58,
        59,
        60,
        62,
        63,
        65,
        66,
        68,
        69,
        71,
        73,
        75,
        78,
        81,
        86,
    ];

    private static readonly short[] From62 =
    [
        14,
        20,
        24,
        26,
        29,
        31,
        33,
        35,
        36,
        38,
        39,
        41,
        42,
        44,
        45,
        46,
        48,
        49,
        50,
        52,
        53,
        54,
        56,
        57,
        58,
        60,
        61,
        63,
        64,
        66,
        67,
        69,
        71,
        73,
        75,
        77,
        79,
        82,
        84,
        88,
        95,
    ];

    /// <summary>
    /// Converts the total response value to the financial well-being score.
    /// </summary>
    /// <param name="total">The sum of the 10 response values (0-40).</param>
    /// <param name="is62Plus">Whether the respondent is 62 or older.</param>
    /// <returns>The financial well-being score.</returns>
    public static short ToScore(int total, bool is62Plus) =>
        is62Plus ? From62[total] : Under62[total];
}
