using Opus127.Dengue.Application.Services;
using Opus127.Dengue.Domain.ValueObjects;

namespace Opus127.Dengue.UnitTests;

public sealed class EpidemiologicalWeekCalculatorTests
{
    private readonly EpidemiologicalWeekCalculator _calculator = new();

    [Theory]
    [InlineData(2024, 12, 28, 2024, 52)]
    [InlineData(2024, 12, 29, 2025, 1)]
    [InlineData(2026, 9, 14, 2026, 38)]
    public void FromDate_ReturnsSundayBasedEpidemiologicalWeek(
        int year,
        int month,
        int day,
        int expectedYear,
        int expectedWeek)
    {
        var result = _calculator.FromDate(new DateOnly(year, month, day));

        Assert.Equal(new EpidemiologicalWeek(expectedYear, expectedWeek), result);
    }

    [Fact]
    public void GetStartDate_ReturnsSunday()
    {
        var result = _calculator.GetStartDate(new EpidemiologicalWeek(2025, 1));

        Assert.Equal(new DateOnly(2024, 12, 29), result);
        Assert.Equal(DayOfWeek.Sunday, result.DayOfWeek);
    }
}
