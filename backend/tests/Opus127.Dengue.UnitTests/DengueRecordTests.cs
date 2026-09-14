using Opus127.Dengue.Domain.Entities;

namespace Opus127.Dengue.UnitTests;

public sealed class DengueRecordTests
{
    [Fact]
    public void Create_RejectsInvalidAlertLevel()
    {
        var action = () => CreateRecord(alertLevel: 5);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Fact]
    public void UpdateFrom_RefreshesMutableSourceData()
    {
        var record = CreateRecord(estimatedCases: 10, notifiedCases: 8, alertLevel: 1);
        var updated = CreateRecord(estimatedCases: 18.5, notifiedCases: 15, alertLevel: 2);

        record.UpdateFrom(updated);

        Assert.Equal(18.5, record.EstimatedCases);
        Assert.Equal(15, record.NotifiedCases);
        Assert.Equal(2, record.AlertLevel);
    }

    private static DengueRecord CreateRecord(
        double estimatedCases = 10,
        int notifiedCases = 8,
        int alertLevel = 1) =>
        DengueRecord.Create(
            3106200,
            2026,
            35,
            new DateOnly(2026, 8, 30),
            estimatedCases,
            estimatedCases,
            estimatedCases,
            notifiedCases,
            alertLevel,
            0.5,
            12.4,
            1.1,
            123,
            "2026-09-11",
            DateTimeOffset.Parse("2026-09-14T12:00:00Z"));
}
