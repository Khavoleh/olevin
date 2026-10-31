using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Olevin.Api.Shared;
using Olevin.Api.Shared.Data;

namespace Olevin.Tests.Shared.Data;

/// <summary>
/// Storing values as <c>jsonb</c>: the shape of the document and how changes are noticed.
/// </summary>
public sealed class JsonbConversionTests
{
    private static readonly Sample Value = new("Rainy day", CurrencyCode.USD, [1, 2], null);

    private static readonly IReadOnlyProperty Property = BuildProperty();

    [Fact]
    public void Keys_AreSnakeCase()
    {
        JsonObject json = ToJson(Value);

        Assert.Equal(
            ["display_name", "currency", "values", "note"],
            json.Select(property => property.Key)
        );
    }

    [Fact]
    public void Enums_AreStoredByName() => Assert.Equal("USD", (string?)ToJson(Value)["currency"]);

    [Fact]
    public void EmptyFields_AreKeptAsNull()
    {
        JsonObject json = ToJson(Value);

        Assert.True(json.ContainsKey("note"));
        Assert.Null(json["note"]);
    }

    [Fact]
    public void Value_SurvivesTheRoundTrip()
    {
        ValueConverter converter = Property.GetValueConverter()!;

        Sample restored = (Sample)
            converter.ConvertFromProvider(converter.ConvertToProvider(Value))!;

        Assert.Equal(Value.DisplayName, restored.DisplayName);
        Assert.Equal(Value.Values, restored.Values);
    }

    [Fact]
    public void Comparer_NoticesAChangeInsideAList()
    {
        ValueComparer comparer = Property.GetValueComparer()!;

        Assert.True(comparer.Equals(Value, Value with { Values = [1, 2] }));
        Assert.False(comparer.Equals(Value, Value with { Values = [1, 3] }));
    }

    [Fact]
    public void Column_IsJsonb() => Assert.Equal("jsonb", Property.GetColumnType());

    private static JsonObject ToJson(Sample value) =>
        JsonNode.Parse((string)Property.GetValueConverter()!.ConvertToProvider(value)!)!.AsObject();

    private static IReadOnlyProperty BuildProperty()
    {
        ModelBuilder builder = new(new ConventionSet());
        builder.Entity<Holder>().Property(holder => holder.Document).HasJsonbConversion();

        return builder.Model.FindEntityType(typeof(Holder))!.FindProperty(nameof(Holder.Document))!;
    }

    private sealed record Sample(
        string DisplayName,
        CurrencyCode Currency,
        IReadOnlyList<int> Values,
        string? Note
    );

    private sealed class Holder
    {
        public Sample Document { get; set; } = null!;
    }
}
