using System.Globalization;
using System.Text;
using System.Text.Json;
using Olevin.Api.Features.FinancialWellness.Slices.Snapshots.Contracts.GetAllSnapshots.DTOs;

namespace Olevin.Api.Features.Settings.Slices.Profile.Handlers.ExportSnapshots;

/// <summary>
/// The rules of the snapshot export.
/// </summary>
public static class ExportSnapshotsRules
{
    private const string LineBreak = "\r\n";
    private const string DateFormat = "yyyy-MM-dd";
    private const string TimestampFormat = "yyyy-MM-dd'T'HH:mm:ss'Z'";

    private static (string Header, Func<UserSnapshotDto, string> Value)[] ColumnsOf(
        IReadOnlyList<UserSnapshotDto> snapshots
    ) =>
        [
            ("id", snapshot => Text(snapshot.Id)),
            ("year", snapshot => Text(snapshot.Year)),
            ("month", snapshot => snapshot.Month),
            (
                "fx_date",
                snapshot => snapshot.FxDate.ToString(DateFormat, CultureInfo.InvariantCulture)
            ),
            ("self_rating", snapshot => Text(snapshot.SelfRating)),
            ("savings_rate", snapshot => Text(snapshot.SavingsRate)),
            ("reserve_months", snapshot => Text(snapshot.ReserveMonths)),
            ("debt_to_income", snapshot => Text(snapshot.DebtToIncome)),
            ("expense_variation", snapshot => Text(snapshot.ExpenseVariation)),
            ("savings_regularity", snapshot => Text(snapshot.SavingsRegularity)),
            ("cash_flow", snapshot => Text(snapshot.CashFlow)),
            ("buffer", snapshot => Text(snapshot.Buffer)),
            ("fuzzy_index", snapshot => Text(snapshot.FuzzyIndex)),
            ("linear_index", snapshot => Text(snapshot.LinearIndex)),
            ("final_index", snapshot => Text(snapshot.FinalIndex)),
            .. ListColumns("income", snapshots, snapshot => snapshot.Income),
            .. MoneyColumns("food", snapshot => snapshot.Expenses.Food),
            .. MoneyColumns("housing", snapshot => snapshot.Expenses.Housing),
            .. MoneyColumns("health", snapshot => snapshot.Expenses.Health),
            .. MoneyColumns("clothing", snapshot => snapshot.Expenses.Clothing),
            .. MoneyColumns("restaurants", snapshot => snapshot.Expenses.Restaurants),
            .. MoneyColumns("transport", snapshot => snapshot.Expenses.Transport),
            .. MoneyColumns("other", snapshot => snapshot.Expenses.Other),
            .. MoneyColumns("saved", snapshot => snapshot.Saved),
            .. ListColumns("reserves", snapshots, snapshot => snapshot.Reserves),
            .. ListColumns("debt_payments", snapshots, snapshot => snapshot.DebtPayments),
            ("model_version", snapshot => snapshot.ModelVersion),
            ("created_at", snapshot => Timestamp(snapshot.CreatedAt)),
            ("updated_at", snapshot => Timestamp(snapshot.UpdatedAt)),
        ];

    /// <summary>
    /// Finds the media type of the exported file.
    /// </summary>
    /// <param name="format">The format of the export.</param>
    /// <returns>The media type.</returns>
    public static string ContentType(ExportFormat format) =>
        format == ExportFormat.Csv ? "text/csv" : "application/json";

    /// <summary>
    /// Finds the name of the exported file.
    /// </summary>
    /// <param name="format">The format of the export.</param>
    /// <param name="exportedOn">The day of the export.</param>
    /// <returns>The file name with the day and the extension of the format.</returns>
    public static string FileName(ExportFormat format, DateOnly exportedOn) =>
        $"olevin-snapshots-{exportedOn.ToString(DateFormat, CultureInfo.InvariantCulture)}.{(format == ExportFormat.Csv ? "csv" : "json")}";

    /// <summary>
    /// Writes the snapshots as an indented JSON array.
    /// </summary>
    /// <param name="snapshots">The snapshots, in the order of the items.</param>
    /// <param name="options">The JSON options of the API.</param>
    /// <returns>The UTF-8 bytes of the array, with the options of the API and a line per value.</returns>
    public static byte[] Json(
        IReadOnlyList<UserSnapshotDto> snapshots,
        JsonSerializerOptions options
    ) =>
        JsonSerializer.SerializeToUtf8Bytes(
            snapshots,
            new JsonSerializerOptions(options) { WriteIndented = true }
        );

    /// <summary>
    /// Writes the snapshots as a CSV table.
    /// </summary>
    /// <param name="snapshots">The snapshots, in the order of the rows.</param>
    /// <returns>
    /// The UTF-8 bytes with a byte order mark so that Excel reads them right: a header and a row per snapshot, with
    /// the rows ended by CRLF. Every amount has an amount column and a currency column; a list of amounts has a pair
    /// of columns per entry, as many as the longest list of the snapshots has, and empty cells for the missing ones.
    /// </returns>
    public static byte[] Csv(IReadOnlyList<UserSnapshotDto> snapshots)
    {
        StringBuilder csv = new();
        (string Header, Func<UserSnapshotDto, string> Value)[] columns = ColumnsOf(snapshots);

        csv.Append(string.Join(',', columns.Select(column => column.Header))).Append(LineBreak);

        foreach (UserSnapshotDto snapshot in snapshots)
        {
            csv.Append(string.Join(',', columns.Select(column => column.Value(snapshot))))
                .Append(LineBreak);
        }

        return [.. Encoding.UTF8.GetPreamble(), .. Encoding.UTF8.GetBytes(csv.ToString())];
    }

    private static (string Header, Func<UserSnapshotDto, string> Value)[] MoneyColumns(
        string name,
        Func<UserSnapshotDto, MoneyDto?> money
    ) =>
        [
            ($"{name}_amount", snapshot => money(snapshot) is { } value ? Text(value.Amount) : ""),
            (
                $"{name}_currency",
                snapshot => money(snapshot) is { } value ? value.Currency.ToString() : ""
            ),
        ];

    private static (string Header, Func<UserSnapshotDto, string> Value)[] ListColumns(
        string name,
        IReadOnlyList<UserSnapshotDto> snapshots,
        Func<UserSnapshotDto, IReadOnlyList<MoneyDto>> list
    ) =>
        [
            .. Enumerable
                .Range(
                    0,
                    Math.Max(
                        1,
                        snapshots.Select(snapshot => list(snapshot).Count).DefaultIfEmpty().Max()
                    )
                )
                .SelectMany(index =>
                    MoneyColumns(
                        $"{name}_{index + 1}",
                        snapshot => list(snapshot).ElementAtOrDefault(index)
                    )
                ),
        ];

    private static string Text(IFormattable value) =>
        value.ToString(null, CultureInfo.InvariantCulture);

    private static string Timestamp(DateTime value) =>
        value.ToUniversalTime().ToString(TimestampFormat, CultureInfo.InvariantCulture);
}
