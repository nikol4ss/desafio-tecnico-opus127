using Microsoft.Extensions.Options;
using Opus127.Dengue.Application.Abstractions;
using Opus127.Dengue.Application.Configuration;
using Opus127.Dengue.Application.Models;
using Opus127.Dengue.Domain.ValueObjects;

namespace Opus127.Dengue.Application.Services;

public sealed class DengueQueryService(
    IDengueRepository repository,
    IOptions<DengueDataOptions> options) : IDengueQueryService
{
    private readonly DengueDataOptions _options = options.Value;

    public async Task<DengueWeekResult?> GetByWeekAsync(
        int year,
        int week,
        CancellationToken cancellationToken)
    {
        var record = await repository.GetByWeekAsync(
            _options.Geocode,
            new EpidemiologicalWeek(year, week),
            cancellationToken);

        return record is null
            ? null
            : new DengueWeekResult(
                record.EpidemiologicalYear,
                record.EpidemiologicalWeek,
                record.WeekStartDate,
                record.EstimatedCases,
                record.NotifiedCases,
                record.AlertLevel);
    }

    public Task<IReadOnlyList<EpidemiologicalWeek>> GetLatestWeeksAsync(
        int count,
        CancellationToken cancellationToken) =>
        repository.GetLatestWeeksAsync(_options.Geocode, count, cancellationToken);
}
