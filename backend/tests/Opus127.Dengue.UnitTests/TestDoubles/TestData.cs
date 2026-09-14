using Opus127.Dengue.Domain.Entities;

namespace Opus127.Dengue.UnitTests.TestDoubles;

internal static class TestData
{
    public static DengueRecord CreateRecord(int year = 2026, int week = 35) =>
        DengueRecord.Create(
            3106200,
            year,
            week,
            new DateOnly(2026, 8, 30).AddDays((week - 35) * 7),
            10,
            8,
            12,
            8,
            1,
            0.5,
            12.4,
            1.1,
            123,
            "2026-09-11",
            DateTimeOffset.Parse("2026-09-14T12:00:00Z"));
}
