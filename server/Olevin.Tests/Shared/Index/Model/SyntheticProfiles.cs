using Olevin.Api.Shared.Index.Model;

namespace Olevin.Tests.Shared.Index.Model;

/// <summary>
/// 15 synthetic profiles from crisis to calm, including the best, the worst, А and Б.
/// </summary>
public static class SyntheticProfiles
{
    public static IReadOnlyList<SyntheticProfile> All { get; } =
    [
        new("Найгірший", new(-0.20, 0, 0.60, 0.50, 0), Zone.Stress, 0, 0),
        new("Криза з боргами", new(-0.15, 0.5, 0.50, 0.35, 0), Zone.Stress, 0, 0),
        new("Від зарплати до зарплати", new(0, 0.5, 0.10, 0.15, 1.0 / 6), Zone.Stress, 0, 31.25),
        new("Тонкий запас", new(0.05, 1.5, 0.30, 0.25, 2.0 / 6), Zone.Stress, 5.46, 24.80),
        new("Профіль А", new(-0.10, 6, 0, 0.05, 1.0 / 6), Zone.Stress, 25.00, 60.00),
        new("Без резерву", new(0.10, 0.5, 0, 0.10, 3.0 / 6), Zone.Stress, 25.00, 53.50),
        new("Профіль Б", new(0.10, 3.5, 0.30, 0.20, 3.0 / 6), Zone.Unstable, 56.21, 49.80),
        new("Молодий фахівець", new(0.15, 1.5, 0, 0.15, 4.0 / 6), Zone.Unstable, 47.70, 63.50),
        new(
            "Високий дохід, нестабільні витрати",
            new(0.25, 2, 0.10, 0.35, 3.0 / 6),
            Zone.Unstable,
            50.00,
            56.00
        ),
        new(
            "Резерв без поточних заощаджень",
            new(0.02, 6, 0.05, 0.10, 3.0 / 6),
            Zone.Unstable,
            57.03,
            68.50
        ),
        new(
            "Пенсіонер із великим резервом",
            new(0.05, 9, 0, 0.05, 3.0 / 6),
            Zone.Unstable,
            62.50,
            72.25
        ),
        new("Іпотека й дисципліна", new(0.20, 4, 0.38, 0.10, 1), Zone.Calm, 75.00, 74.35),
        new("Комфортний", new(0.25, 5, 0.10, 0.15, 4.0 / 6), Zone.Calm, 86.30, 87.25),
        new("Середній клас", new(0.20, 4, 0.15, 0.10, 5.0 / 6), Zone.Calm, 100, 90.00),
        new("Найкращий", new(0.40, 9, 0, 0, 1), Zone.Calm, 100, 100),
    ];
}
