using Opus127.Dengue.Domain.Entities;
using Opus127.Dengue.Domain.ValueObjects;

namespace Opus127.Dengue.Application.Abstractions;

public interface IDengueRepository
{
    Task<DengueRecord?> GetByWeekAsync(
        int geocode,
        EpidemiologicalWeek week,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<EpidemiologicalWeek>> GetLatestWeeksAsync(
        int geocode,
        int count,
        CancellationToken cancellationToken);

    Task UpsertAsync(
        IReadOnlyCollection<DengueRecord> records,
        CancellationToken cancellationToken);
}
