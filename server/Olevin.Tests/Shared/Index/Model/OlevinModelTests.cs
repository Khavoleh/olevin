using Olevin.Api.Shared.Index.Fuzzy;
using Olevin.Api.Shared.Index.Indicators;
using Olevin.Api.Shared.Index.Model;

namespace Olevin.Tests.Shared.Index.Model;

public sealed class OlevinModelTests
{
    [Fact]
    public void Model_Has45RulesNumbered1To45()
    {
        int[] numbers =
        [
            .. OlevinModel.CashFlowSystem.Rules.Select(r => r.Number),
            .. OlevinModel.BufferSystem.Rules.Select(r => r.Number),
            .. OlevinModel.IndexSystem.Rules.Select(r => r.Number),
        ];

        Assert.Equal(Enumerable.Range(1, 45), numbers);
    }

    [Fact]
    public void CashFlowRules_FollowTheirNumberingAndTable()
    {
        int[,] table =
        {
            { 0, 0, 0 },
            { 1, 1, 0 },
            { 2, 1, 1 },
        };

        foreach (FuzzyRule rule in OlevinModel.CashFlowSystem.Rules)
        {
            (int sr, int dti) = (rule.Conditions[0], rule.Conditions[1]);

            Assert.Equal(1 + (3 * sr) + dti, rule.Number);
            Assert.Equal(table[sr, dti], rule.Conclusion);
        }
    }

    [Fact]
    public void BufferRules_FollowTheirNumberingAndTable()
    {
        int[,] table =
        {
            { 0, 0, 0 },
            { 2, 1, 0 },
            { 2, 2, 1 },
        };

        foreach (FuzzyRule rule in OlevinModel.BufferSystem.Rules)
        {
            (int r, int cv) = (rule.Conditions[0], rule.Conditions[1]);

            Assert.Equal(10 + (3 * r) + cv, rule.Number);
            Assert.Equal(table[r, cv], rule.Conclusion);
        }
    }

    [Fact]
    public void IndexRules_FollowTheFormulaForTheOutputTerm()
    {
        foreach (FuzzyRule rule in OlevinModel.IndexSystem.Rules)
        {
            (int flow, int buffer, int rg) = (
                rule.Conditions[0],
                rule.Conditions[1],
                rule.Conditions[2]
            );
            int sum = flow + buffer;
            int expected = rg switch
            {
                2 => sum,
                1 => Math.Min(sum, 3),
                _ => Math.Max(sum - 1, 0),
            };

            Assert.Equal(19 + (9 * flow) + (3 * buffer) + rg, rule.Number);
            Assert.Equal(expected, rule.Conclusion);
        }
    }

    public static TheoryData<string> Systems => ["F1 cash flow", "F2 buffer", "F3 index"];

    [Theory]
    [MemberData(nameof(Systems))]
    public void RuleBases_AreCompleteAndUnique(string name)
    {
        MamdaniSystem system = AllSystems().Single(s => s.Name == name);
        int combinations = system.Inputs.Aggregate(1, (count, input) => count * input.Terms.Count);

        Assert.Equal(combinations, system.Rules.Count);
        Assert.Equal(
            combinations,
            system.Rules.Select(r => string.Join(',', r.Conditions)).Distinct().Count()
        );
    }

    [Theory]
    [MemberData(nameof(Systems))]
    public void RuleBases_AreMonotone_ABetterInputTermNeverGivesAWorseConclusion(string name)
    {
        MamdaniSystem system = AllSystems().Single(s => s.Name == name);

        // Terms that are better have a higher index, except for DTI and CV, where lower is better.
        bool[] lowerIsBetter =
        [
            .. system.Inputs.Select(input =>
                input == OlevinModel.DebtToIncome || input == OlevinModel.ExpenseVariation
            ),
        ];

        foreach (FuzzyRule rule in system.Rules)
        {
            for (int i = 0; i < rule.Conditions.Count; i++)
            {
                int better = rule.Conditions[i] + (lowerIsBetter[i] ? -1 : 1);
                FuzzyRule? neighbour = system.Rules.SingleOrDefault(other =>
                    other.Conditions[i] == better
                    && other
                        .Conditions.Where((_, j) => j != i)
                        .SequenceEqual(rule.Conditions.Where((_, j) => j != i))
                );

                if (neighbour is not null)
                {
                    Assert.True(
                        neighbour.Conclusion >= rule.Conclusion,
                        $"Rule {neighbour.Number} is worse than rule {rule.Number}."
                    );
                }
            }
        }
    }

    [Fact]
    public void BestTerms_AreTheMostFavourableOnes()
    {
        Assert.Equal("high", BestTermName(Indicator.SavingsRate));
        Assert.Equal("large", BestTermName(Indicator.Reserve));
        Assert.Equal("low", BestTermName(Indicator.DebtToIncome));
        Assert.Equal("stable", BestTermName(Indicator.ExpenseVariation));
        Assert.Equal("regularly", BestTermName(Indicator.SavingsRegularity));
    }

    private static string BestTermName(Indicator indicator) =>
        OlevinModel.Variable(indicator).Terms[OlevinModel.BestTerm(indicator)].Name;

    private static MamdaniSystem[] AllSystems() =>
        [OlevinModel.CashFlowSystem, OlevinModel.BufferSystem, OlevinModel.IndexSystem];
}
