using Microsoft.EntityFrameworkCore;
using Opus127.Dengue.Application.Abstractions;
using Opus127.Dengue.Domain.Entities;
using Opus127.Dengue.Domain.ValueObjects;

namespace Opus127.Dengue.Infrastructure.Persistence.Repositories;

public sealed class DengueRepository(DengueDbContext dbContext) : IDengueRepository
{
    public Task<DengueRecord?> GetByWeekAsync(
        int geocode,
        EpidemiologicalWeek week,
        CancellationToken cancellationToken) =>
        dbContext.DengueRecords
            .AsNoTracking()
            .SingleOrDefaultAsync(
                record => record.Geocode == geocode &&
                          record.EpidemiologicalYear == week.Year &&
                          record.EpidemiologicalWeek == week.Week,
                cancellationToken);

    public async Task<IReadOnlyList<EpidemiologicalWeek>> GetLatestWeeksAsync(
        int geocode,
        int count,
        CancellationToken cancellationToken)
    {
        var weeks = await dbContext.DengueRecords
            .AsNoTracking()
            .Where(record => record.Geocode == geocode)
            .OrderByDescending(record => record.EpidemiologicalYear)
            .ThenByDescending(record => record.EpidemiologicalWeek)
            .Select(record => new { record.EpidemiologicalYear, record.EpidemiologicalWeek })
            .Take(count)
            .ToListAsync(cancellationToken);

        return weeks
            .Select(week => new EpidemiologicalWeek(
                week.EpidemiologicalYear,
                week.EpidemiologicalWeek))
            .ToArray();
    }

    public async Task UpsertAsync(
        IReadOnlyCollection<DengueRecord> records,
        CancellationToken cancellationToken)
    {
        if (records.Count == 0)
        {
            return;
        }

        var geocodes = records.Select(record => record.Geocode).Distinct().ToArray();
        var minimumYear = records.Min(record => record.EpidemiologicalYear);
        var maximumYear = records.Max(record => record.EpidemiologicalYear);
        var existingRecords = await dbContext.DengueRecords
            .Where(record => geocodes.Contains(record.Geocode) &&
                             record.EpidemiologicalYear >= minimumYear &&
                             record.EpidemiologicalYear <= maximumYear)
            .ToListAsync(cancellationToken);
        var existingByWeek = existingRecords.ToDictionary(
            record => (record.Geocode, record.EpidemiologicalYear, record.EpidemiologicalWeek));

        foreach (var record in records)
        {
            var key = (record.Geocode, record.EpidemiologicalYear, record.EpidemiologicalWeek);

            if (existingByWeek.TryGetValue(key, out var existingRecord))
            {
                existingRecord.UpdateFrom(record);
            }
            else
            {
                dbContext.DengueRecords.Add(record);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
