using Olevin.Api.Features.Settings.Slices.Onboarding.Handlers.SubmitCfpbResponse.Enums;

namespace Olevin.Api.Features.Settings.Slices.Onboarding.Handlers.SubmitCfpbResponse;

/// <summary>
/// The answers of a user to the CFPB Financial Well-Being Scale, as chosen in the questionnaire.
/// </summary>
/// <param name="CouldHandleMajorExpense">1. I could handle a major unexpected expense.</param>
/// <param name="SecuringFinancialFuture">2. I am securing my financial future.</param>
/// <param name="NeverHaveThingsWanted">
/// 3. Because of my money situation, I feel like I will never have the things I want in life.
/// </param>
/// <param name="EnjoyLifeManagingMoney">4. I can enjoy life because of the way I'm managing my money.</param>
/// <param name="JustGettingBy">5. I am just getting by financially.</param>
/// <param name="MoneyWontLast">6. I am concerned that the money I have or will save won't last.</param>
/// <param name="GiftStrainsFinances">
/// 7. Giving a gift for a wedding, birthday or other occasion would put a strain on my finances for the month.
/// </param>
/// <param name="MoneyLeftOver">8. I have money left over at the end of the month.</param>
/// <param name="BehindWithFinances">9. I am behind with my finances.</param>
/// <param name="FinancesControlLife">10. My finances control my life.</param>
/// <param name="Is62Plus">Whether the user is 62 or older.</param>
public sealed record SubmitCfpbResponseRequest(
    CfpbDescription CouldHandleMajorExpense,
    CfpbDescription SecuringFinancialFuture,
    CfpbDescription NeverHaveThingsWanted,
    CfpbDescription EnjoyLifeManagingMoney,
    CfpbDescription JustGettingBy,
    CfpbDescription MoneyWontLast,
    CfpbFrequency GiftStrainsFinances,
    CfpbFrequency MoneyLeftOver,
    CfpbFrequency BehindWithFinances,
    CfpbFrequency FinancesControlLife,
    bool Is62Plus
);
