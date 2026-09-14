using Opus127.Dengue.Application.Models;
using Opus127.Dengue.Domain.ValueObjects;

namespace Opus127.Dengue.Application.Abstractions;

public interface IDengueDataSource
{
    Task<IReadOnlyCollection<DengueSourceRecord>> FetchAsync(
        int geocode,
        EpidemiologicalWeek start,
        EpidemiologicalWeek end,
        CancellationToken cancellationToken);
}
