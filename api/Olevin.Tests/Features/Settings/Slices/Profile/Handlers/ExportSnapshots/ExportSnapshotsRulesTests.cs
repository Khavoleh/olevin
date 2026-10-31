using System.Text;
using System.Text.Json;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Contracts.GetAllSnapshots.DTOs;
using Olevin.Api.Features.Settings.Slices.Profile.Handlers.ExportSnapshots;
using Olevin.Api.Shared;

namespace Olevin.Tests.Features.Settings.Slices.Profile.Handlers.ExportSnapshots;

public sealed class ExportSnapshotsRulesTests
{
    private static readonly byte[] ByteOrderMark = [0xEF, 0xBB, 0xBF];

    private static MoneyDto Usd(decimal amount) => new(amount, CurrencyCode.USD);

    private static UserSnapshotDto Snapshot() =>
        new(
            new Guid("0197a3b2-1c4d-7e5f-8a9b-0c1d2e3f4a5b"),
            "March",
            2026,
            new DateOnly(2026, 3, 15),
            4,
            0.12,
            3.5,
            0.2,
            0.15,
            0.75,
            61.23,
            48.51,
            55.1,
            52.4,
            55.07,
            [Usd(5000.50m), new MoneyDto(200m, CurrencyCode.EUR)],
            new ExpensesDto(Usd(400), Usd(1200), Usd(50), Usd(80), Usd(150), Usd(90), Usd(30)),
            Usd(700),
            [Usd(9000)],
            [],
            "1.0",
            new DateTime(2026, 3, 15, 8, 30, 5, DateTimeKind.Utc),
            new DateTime(2026, 3, 16, 9, 0, 0, DateTimeKind.Utc)
        );

    private static string[] Lines(byte[] csv) =>
        Encoding.UTF8.GetString(csv, 3, csv.Length - 3).Split("\r\n", StringSplitOptions.None);

    [Fact]
    public void Json_PutsEveryValueOnItsOwnIndentedLine()
    {
        string json = Encoding.UTF8.GetString(
            ExportSnapshotsRules.Json(
                [Snapshot()],
                new JsonSerializerOptions(JsonSerializerDefaults.Web)
            )
        );
        string[] lines = json.Split('\n');

        Assert.True(lines.Length > 20);
        Assert.Equal("[", lines[0].TrimEnd());
        Assert.Contains("    \"year\": 2026,", lines);
    }

    [Fact]
    public void Json_KeepsTheOptionsOfTheApi()
    {
        string json = Encoding.UTF8.GetString(
            ExportSnapshotsRules.Json(
                [Snapshot()],
                new JsonSerializerOptions(JsonSerializerDefaults.Web)
            )
        );

        Assert.Contains("\"fxDate\": \"2026-03-15\"", json);
        Assert.DoesNotContain("FxDate", json);
    }

    [Fact]
    public void Json_WithoutSnapshots_IsAnEmptyArray() =>
        Assert.Equal(
            "[]",
            Encoding.UTF8.GetString(
                ExportSnapshotsRules.Json([], new JsonSerializerOptions(JsonSerializerDefaults.Web))
            )
        );

    [Fact]
    public void Csv_StartsWithTheByteOrderMark() =>
        Assert.Equal(ByteOrderMark, ExportSnapshotsRules.Csv([Snapshot()]).Take(3));

    [Fact]
    public void Csv_WithoutSnapshots_HasOnlyTheHeader()
    {
        string[] lines = Lines(ExportSnapshotsRules.Csv([]));

        Assert.Equal(2, lines.Length);
        Assert.StartsWith("id,year,month,fx_date,self_rating,savings_rate,", lines[0]);
        Assert.Equal(string.Empty, lines[1]);
    }

    [Fact]
    public void Csv_HasARowPerSnapshotWithAsManyCellsAsTheHeader()
    {
        string[] lines = Lines(ExportSnapshotsRules.Csv([Snapshot(), Snapshot()]));

        Assert.Equal(4, lines.Length);
        Assert.Equal(lines[0].Split(',').Length, lines[1].Split(',').Length);
        Assert.Equal(lines[1], lines[2]);
    }

    [Fact]
    public void Csv_GivesEveryAmountItsOwnAmountAndCurrencyColumns()
    {
        string[] header = Lines(ExportSnapshotsRules.Csv([Snapshot()]))[0].Split(',');

        Assert.All(
            header.Where(name => name.EndsWith("_amount", StringComparison.Ordinal)),
            name =>
                Assert.Contains(
                    name.Replace("_amount", "_currency", StringComparison.Ordinal),
                    header
                )
        );
        Assert.Contains("income_2_amount", header);
        Assert.DoesNotContain("income_3_amount", header);
        Assert.Contains("debt_payments_1_currency", header);
    }

    [Fact]
    public void Csv_LeavesTheMissingEntriesOfAListEmpty()
    {
        UserSnapshotDto shorter = Snapshot() with { Income = [Usd(1)] };
        string[] lines = Lines(ExportSnapshotsRules.Csv([Snapshot(), shorter]));
        string[] header = lines[0].Split(',');
        int second = Array.IndexOf(header, "income_2_amount");

        Assert.Equal(header.Length, lines[2].Split(',').Length);
        Assert.Equal(string.Empty, lines[2].Split(',')[second]);
        Assert.Equal(string.Empty, lines[2].Split(',')[second + 1]);
    }

    [Fact]
    public void Csv_WritesTheSnapshotAsEntered() =>
        Assert.Equal(
            "0197a3b2-1c4d-7e5f-8a9b-0c1d2e3f4a5b,2026,March,2026-03-15,4,0.12,3.5,0.2,0.15,0.75,"
                + "61.23,48.51,55.1,52.4,55.07,5000.50,USD,200,EUR,"
                + "400,USD,1200,USD,50,USD,80,USD,150,USD,90,USD,30,USD,700,USD,"
                + "9000,USD,,,1.0,2026-03-15T08:30:05Z,2026-03-16T09:00:00Z",
            Lines(ExportSnapshotsRules.Csv([Snapshot()]))[1]
        );

    [Theory]
    [InlineData(ExportFormat.Json, "application/json")]
    [InlineData(ExportFormat.Csv, "text/csv")]
    public void ContentType_FollowsTheFormat(ExportFormat format, string expected) =>
        Assert.Equal(expected, ExportSnapshotsRules.ContentType(format));

    [Theory]
    [InlineData(ExportFormat.Json, "olevin-snapshots-2026-10-25.json")]
    [InlineData(ExportFormat.Csv, "olevin-snapshots-2026-10-25.csv")]
    public void FileName_HasTheDayAndTheExtension(ExportFormat format, string expected) =>
        Assert.Equal(expected, ExportSnapshotsRules.FileName(format, new DateOnly(2026, 10, 25)));
}
