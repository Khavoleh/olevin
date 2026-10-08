using Olevin.Api.Shared.Index.Fuzzy;
using Olevin.Api.Shared.Index.Indicators;

namespace Olevin.Api.Shared.Index.Model;

/// <summary>
/// The hierarchical Mamdani model of the financial calm index:
/// <c>I_fuzzy = F₃(F₁(SR, DTI), F₂(R, CV), RG)</c> with 9 + 9 + 27 = 45 numbered rules.
/// </summary>
public static class OlevinModel
{
    // Term indexes of the inputs (table "terms and membership functions").
    private const int SrNegative = 0;
    private const int SrLow = 1;
    private const int SrHigh = 2;

    private const int RSmall = 0;
    private const int RSufficient = 1;
    private const int RLarge = 2;

    private const int DtiLow = 0;
    private const int DtiModerate = 1;
    private const int DtiHigh = 2;

    private const int CvStable = 0;
    private const int CvModerate = 1;
    private const int CvUnstable = 2;

    private const int RgRarely = 0;
    private const int RgSometimes = 1;
    private const int RgRegularly = 2;

    // Term indexes of "cash flow" and "buffer".
    private const int Weak = 0;
    private const int Moderate = 1;
    private const int Strong = 2;

    // Term indexes of the final output.
    private const int Stress = 0;
    private const int Strain = 1;
    private const int Unstable = 2;
    private const int Stable = 3;
    private const int Calm = 4;

    /// <summary>
    /// Gets the savings rate SR on [−0.2; 0.4].
    /// </summary>
    public static LinguisticVariable SavingsRate { get; } =
        new(
            "SR",
            -0.2,
            0.4,
            [
                new("negative", MembershipFunction.Trapezoid(-0.2, -0.2, 0, 0.1)),
                new("low", MembershipFunction.Triangle(0, 0.1, 0.2)),
                new("high", MembershipFunction.Trapezoid(0.1, 0.2, 0.4, 0.4)),
            ]
        );

    /// <summary>
    /// Gets the reserve R in months on [0; 9].
    /// </summary>
    public static LinguisticVariable Reserve { get; } =
        new(
            "R",
            0,
            9,
            [
                new("small", MembershipFunction.Trapezoid(0, 0, 1, 3)),
                new("sufficient", MembershipFunction.Triangle(1, 3, 6)),
                new("large", MembershipFunction.Trapezoid(3, 6, 9, 9)),
            ]
        );

    /// <summary>
    /// Gets the debt-to-income DTI on [0; 0.6].
    /// </summary>
    public static LinguisticVariable DebtToIncome { get; } =
        new(
            "DTI",
            0,
            0.6,
            [
                new("low", MembershipFunction.Trapezoid(0, 0, 0.2, 0.36)),
                new("moderate", MembershipFunction.Triangle(0.2, 0.36, 0.43)),
                new("high", MembershipFunction.Trapezoid(0.36, 0.43, 0.6, 0.6)),
            ]
        );

    /// <summary>
    /// Gets the expense variation CV on [0; 0.5].
    /// </summary>
    public static LinguisticVariable ExpenseVariation { get; } =
        new(
            "CV",
            0,
            0.5,
            [
                new("stable", MembershipFunction.Trapezoid(0, 0, 0.1, 0.2)),
                new("moderate", MembershipFunction.Triangle(0.1, 0.2, 0.3)),
                new("unstable", MembershipFunction.Trapezoid(0.2, 0.3, 0.5, 0.5)),
            ]
        );

    /// <summary>
    /// Gets the savings regularity RG on [0; 1].
    /// </summary>
    public static LinguisticVariable SavingsRegularity { get; } =
        new(
            "RG",
            0,
            1,
            [
                new("rarely", MembershipFunction.Trapezoid(0, 0, 0.3, 0.5)),
                new("sometimes", MembershipFunction.Triangle(0.3, 0.5, 0.8)),
                new("regularly", MembershipFunction.Trapezoid(0.5, 0.8, 1, 1)),
            ]
        );

    /// <summary>
    /// Gets the intermediate variable "cash flow" on [0; 100].
    /// </summary>
    public static LinguisticVariable CashFlow { get; } = Subsystem("cash flow");

    /// <summary>
    /// Gets the intermediate variable "financial buffer" on [0; 100].
    /// </summary>
    public static LinguisticVariable Buffer { get; } = Subsystem("buffer");

    /// <summary>
    /// Gets the final output on [0; 100]. Symmetric triangles reach past the range, so a single "stress" term gives
    /// exactly 0 and a single "calm" term exactly 100.
    /// </summary>
    public static LinguisticVariable Index { get; } =
        new(
            "I_fuzzy",
            0,
            100,
            [
                new("stress", MembershipFunction.Triangle(-25, 0, 25)),
                new("strain", MembershipFunction.Triangle(0, 25, 50)),
                new("unstable", MembershipFunction.Triangle(25, 50, 75)),
                new("stable", MembershipFunction.Triangle(50, 75, 100)),
                new("calm", MembershipFunction.Triangle(75, 100, 125)),
            ]
        );

    /// <summary>
    /// Gets F₁ "cash flow" with rules 1–9: № = 1 + 3·i_SR + i_DTI.
    /// </summary>
    public static MamdaniSystem CashFlowSystem { get; } =
        new(
            "F1 cash flow",
            [SavingsRate, DebtToIncome],
            CashFlow,
            [
                new(1, [SrNegative, DtiLow], Weak),
                new(2, [SrNegative, DtiModerate], Weak),
                new(3, [SrNegative, DtiHigh], Weak),
                new(4, [SrLow, DtiLow], Moderate),
                new(5, [SrLow, DtiModerate], Moderate),
                new(6, [SrLow, DtiHigh], Weak),
                new(7, [SrHigh, DtiLow], Strong),
                new(8, [SrHigh, DtiModerate], Moderate),
                new(9, [SrHigh, DtiHigh], Moderate),
            ],
            -50,
            150
        );

    /// <summary>
    /// Gets F₂ "financial buffer" with rules 10–18: № = 10 + 3·i_R + i_CV.
    /// </summary>
    public static MamdaniSystem BufferSystem { get; } =
        new(
            "F2 buffer",
            [Reserve, ExpenseVariation],
            Buffer,
            [
                new(10, [RSmall, CvStable], Weak),
                new(11, [RSmall, CvModerate], Weak),
                new(12, [RSmall, CvUnstable], Weak),
                new(13, [RSufficient, CvStable], Strong),
                new(14, [RSufficient, CvModerate], Moderate),
                new(15, [RSufficient, CvUnstable], Weak),
                new(16, [RLarge, CvStable], Strong),
                new(17, [RLarge, CvModerate], Strong),
                new(18, [RLarge, CvUnstable], Moderate),
            ],
            -50,
            150
        );

    /// <summary>
    /// Gets F₃, the final system, with rules 19–45: № = 19 + 9·r_flow + 3·r_buffer + i_RG.
    /// </summary>
    /// <remarks>
    /// The output term is k = r_flow + r_buffer when saving regularly, min(k, 3) when saving sometimes and
    /// max(k − 1, 0) when saving rarely.
    /// </remarks>
    public static MamdaniSystem IndexSystem { get; } =
        new(
            "F3 index",
            [CashFlow, Buffer, SavingsRegularity],
            Index,
            [
                new(19, [Weak, Weak, RgRarely], Stress),
                new(20, [Weak, Weak, RgSometimes], Stress),
                new(21, [Weak, Weak, RgRegularly], Stress),
                new(22, [Weak, Moderate, RgRarely], Stress),
                new(23, [Weak, Moderate, RgSometimes], Strain),
                new(24, [Weak, Moderate, RgRegularly], Strain),
                new(25, [Weak, Strong, RgRarely], Strain),
                new(26, [Weak, Strong, RgSometimes], Unstable),
                new(27, [Weak, Strong, RgRegularly], Unstable),
                new(28, [Moderate, Weak, RgRarely], Stress),
                new(29, [Moderate, Weak, RgSometimes], Strain),
                new(30, [Moderate, Weak, RgRegularly], Strain),
                new(31, [Moderate, Moderate, RgRarely], Strain),
                new(32, [Moderate, Moderate, RgSometimes], Unstable),
                new(33, [Moderate, Moderate, RgRegularly], Unstable),
                new(34, [Moderate, Strong, RgRarely], Unstable),
                new(35, [Moderate, Strong, RgSometimes], Stable),
                new(36, [Moderate, Strong, RgRegularly], Stable),
                new(37, [Strong, Weak, RgRarely], Strain),
                new(38, [Strong, Weak, RgSometimes], Unstable),
                new(39, [Strong, Weak, RgRegularly], Unstable),
                new(40, [Strong, Moderate, RgRarely], Unstable),
                new(41, [Strong, Moderate, RgSometimes], Stable),
                new(42, [Strong, Moderate, RgRegularly], Stable),
                new(43, [Strong, Strong, RgRarely], Stable),
                new(44, [Strong, Strong, RgSometimes], Stable),
                new(45, [Strong, Strong, RgRegularly], Calm),
            ],
            -25,
            125
        );

    /// <summary>
    /// Gets the input variable of an indicator.
    /// </summary>
    /// <param name="indicator">The indicator.</param>
    /// <returns>Its linguistic variable.</returns>
    public static LinguisticVariable Variable(Indicator indicator)
    {
        return indicator switch
        {
            Indicator.SavingsRate => SavingsRate,
            Indicator.Reserve => Reserve,
            Indicator.DebtToIncome => DebtToIncome,
            Indicator.ExpenseVariation => ExpenseVariation,
            Indicator.SavingsRegularity => SavingsRegularity,
            _ => throw new ArgumentOutOfRangeException(nameof(indicator), indicator, null),
        };
    }

    /// <summary>
    /// Gets the index of the most favourable term of an indicator.
    /// </summary>
    /// <param name="indicator">The indicator.</param>
    /// <returns>The term index: the lowest one for DTI and CV, the highest one for the rest.</returns>
    public static int BestTerm(Indicator indicator)
    {
        return indicator switch
        {
            Indicator.SavingsRate => SrHigh,
            Indicator.Reserve => RLarge,
            Indicator.DebtToIncome => DtiLow,
            Indicator.ExpenseVariation => CvStable,
            Indicator.SavingsRegularity => RgRegularly,
            _ => throw new ArgumentOutOfRangeException(nameof(indicator), indicator, null),
        };
    }

    /// <summary>
    /// Runs the whole hierarchy. The indicators are limited to their ranges first.
    /// </summary>
    /// <param name="indicators">The indicators.</param>
    /// <returns>The results of F₁, F₂ and F₃.</returns>
    public static FuzzyEvaluation Evaluate(IndicatorVector indicators)
    {
        IndicatorVector x = indicators.Clamp();

        MamdaniResult cashFlow = CashFlowSystem.Evaluate(x.SavingsRate, x.DebtToIncome);
        MamdaniResult buffer = BufferSystem.Evaluate(x.Reserve, x.ExpenseVariation);
        MamdaniResult index = IndexSystem.Evaluate(
            cashFlow.Output,
            buffer.Output,
            x.SavingsRegularity
        );

        return new FuzzyEvaluation(cashFlow, buffer, index);
    }

    private static LinguisticVariable Subsystem(string name)
    {
        return new(
            name,
            0,
            100,
            [
                new("weak", MembershipFunction.Triangle(-50, 0, 50)),
                new("moderate", MembershipFunction.Triangle(0, 50, 100)),
                new("strong", MembershipFunction.Triangle(50, 100, 150)),
            ]
        );
    }
}
