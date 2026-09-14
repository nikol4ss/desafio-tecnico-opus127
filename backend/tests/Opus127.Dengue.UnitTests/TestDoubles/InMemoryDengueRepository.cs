using Opus127.Dengue.Application.Abstractions;
using Opus127.Dengue.Domain.Entities;
using Opus127.Dengue.Domain.ValueObjects;

namespace Opus127.Dengue.UnitTests.TestDoubles;

internal sealed class InMemoryDengueRepository : IDengueRepository
{
    public List<DengueRecord> Records { get; } = [];

    public Task<DengueRecord?> GetByWeekAsync(
        int geocode,
        EpidemiologicalWeek week,
        CancellationToken cancellationToken) =>
        Task.FromResult(Records.SingleOrDefault(record =>
            record.Geocode == geocode &&
            record.EpidemiologicalYear == week.Year &&
            record.EpidemiologicalWeek == week.Week));

    public Task<IReadOnlyList<EpidemiologicalWeek>> GetLatestWeeksAsync(
        int geocode,
        int count,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<EpidemiologicalWeek> result = Records
            .Where(record => record.Geocode == geocode)
            .OrderByDescending(record => record.EpidemiologicalYear)
            .ThenByDescending(record => record.EpidemiologicalWeek)
            .Take(count)
            .Select(record => new EpidemiologicalWeek(
                record.EpidemiologicalYear,
                record.EpidemiologicalWeek))
            .ToArray();

        return Task.FromResult(result);
    }

    public Task UpsertAsync(
        IReadOnlyCollection<DengueRecord> records,
        CancellationToken cancellationToken)
    {
        foreach (var record in records)
        {
            var existing = Records.SingleOrDefault(item =>
                item.Geocode == record.Geocode &&
                item.EpidemiologicalYear == record.EpidemiologicalYear &&
                item.EpidemiologicalWeek == record.EpidemiologicalWeek);

            if (existing is null)
            {
                Records.Add(record);
            }
            else
            {
                existing.UpdateFrom(record);
            }
        }

        return Task.CompletedTask;
    }
}
