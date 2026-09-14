using Opus127.Dengue.Application.Models;
using Opus127.Dengue.Domain.ValueObjects;

namespace Opus127.Dengue.Application.Abstractions;

public interface IDengueQueryService
{
    Task<DengueWeekResult?> GetByWeekAsync(
        int year,
        int week,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<EpidemiologicalWeek>> GetLatestWeeksAsync(
        int count,
        CancellationToken cancellationToken);
}
