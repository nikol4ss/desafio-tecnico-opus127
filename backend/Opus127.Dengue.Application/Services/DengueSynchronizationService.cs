using Microsoft.Extensions.Options;
using Opus127.Dengue.Application.Abstractions;
using Opus127.Dengue.Application.Configuration;
using Opus127.Dengue.Application.Models;
using Opus127.Dengue.Domain.Entities;

namespace Opus127.Dengue.Application.Services;

public sealed class DengueSynchronizationService(
    IDengueDataSource dataSource,
    IDengueRepository repository,
    IEpidemiologicalWeekCalculator weekCalculator,
    IOptions<DengueDataOptions> options,
    TimeProvider timeProvider) : IDengueSynchronizationService
{
    private readonly DengueDataOptions _options = options.Value;

    public async Task<DengueSyncResult> SynchronizeLastSixMonthsAsync(
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        var start = weekCalculator.FromDate(today.AddMonths(-6));
        var end = weekCalculator.FromDate(today);
        var sourceRecords = await dataSource.FetchAsync(
            _options.Geocode,
            start,
            end,
            cancellationToken);
        var syncedAtUtc = timeProvider.GetUtcNow();
        var records = sourceRecords
            .Select(item => DengueRecord.Create(
                _options.Geocode,
                item.EpidemiologicalYear,
                item.EpidemiologicalWeek,
                item.WeekStartDate,
                item.EstimatedCases,
                item.EstimatedCasesMinimum,
                item.EstimatedCasesMaximum,
                item.NotifiedCases,
                item.AlertLevel,
                item.ProbabilityRtAboveOne,
                item.EstimatedIncidencePer100K,
                item.ReproductionNumber,
                item.SourceRecordId,
                item.ModelVersion,
                syncedAtUtc))
            .ToArray();

        await repository.UpsertAsync(records, cancellationToken);

        return new DengueSyncResult(start, end, records.Length);
    }
}
