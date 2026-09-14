using Microsoft.Extensions.Options;
using Opus127.Dengue.Application.Configuration;
using Opus127.Dengue.Application.Services;
using Opus127.Dengue.Domain.ValueObjects;
using Opus127.Dengue.UnitTests.TestDoubles;

namespace Opus127.Dengue.UnitTests;

public sealed class DengueQueryServiceTests
{
    [Fact]
    public async Task GetByWeekAsync_MapsStoredRecord()
    {
        var repository = new InMemoryDengueRepository();
        repository.Records.Add(TestData.CreateRecord());
        var service = new DengueQueryService(repository, CreateOptions());

        var result = await service.GetByWeekAsync(2026, 35, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(35, result.EpidemiologicalWeek);
        Assert.Equal(10, result.EstimatedCases);
        Assert.Equal(8, result.NotifiedCases);
    }

    [Fact]
    public async Task GetLatestWeeksAsync_ReturnsRepositoryOrder()
    {
        var repository = new InMemoryDengueRepository();
        repository.Records.Add(TestData.CreateRecord(2026, 34));
        repository.Records.Add(TestData.CreateRecord(2026, 35));
        var service = new DengueQueryService(repository, CreateOptions());

        var result = await service.GetLatestWeeksAsync(2, CancellationToken.None);

        Assert.Equal(
            [new EpidemiologicalWeek(2026, 35), new EpidemiologicalWeek(2026, 34)],
            result);
    }

    private static IOptions<DengueDataOptions> CreateOptions() =>
        Options.Create(new DengueDataOptions { Geocode = 3106200 });
}
