using Microsoft.Extensions.Options;
using Opus127.Dengue.Application.Configuration;
using Opus127.Dengue.Application.Models;
using Opus127.Dengue.Application.Services;
using Opus127.Dengue.Domain.ValueObjects;
using Opus127.Dengue.UnitTests.TestDoubles;

namespace Opus127.Dengue.UnitTests;

public sealed class DengueSynchronizationServiceTests
{
    [Fact]
    public async Task SynchronizeLastSixMonthsAsync_CalculatesRangeAndPersistsRecords()
    {
        var source = new StubDengueDataSource(
            [new DengueSourceRecord(
                2026,
                35,
                new DateOnly(2026, 8, 30),
                12.5,
                10,
                15,
                9,
                2,
                0.8,
                4.2,
                1.05,
                123,
                "2026-09-11")]);
        var repository = new InMemoryDengueRepository();
        var service = new DengueSynchronizationService(
            source,
            repository,
            new EpidemiologicalWeekCalculator(),
            Options.Create(new DengueDataOptions { Geocode = 3106200 }),
            new FixedTimeProvider(new DateTimeOffset(2026, 9, 14, 12, 0, 0, TimeSpan.Zero)));

        var result = await service.SynchronizeLastSixMonthsAsync(CancellationToken.None);

        Assert.Equal(new EpidemiologicalWeek(2026, 11), result.Start);
        Assert.Equal(new EpidemiologicalWeek(2026, 38), result.End);
        Assert.Equal(1, result.ImportedRecords);
        Assert.Single(repository.Records);
        Assert.Equal(3106200, repository.Records[0].Geocode);
        Assert.Equal(new EpidemiologicalWeek(2026, 11), source.RequestedStart);
        Assert.Equal(new EpidemiologicalWeek(2026, 38), source.RequestedEnd);
    }
}
