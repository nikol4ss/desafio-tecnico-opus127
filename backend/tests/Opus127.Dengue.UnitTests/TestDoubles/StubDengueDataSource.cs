using Opus127.Dengue.Application.Abstractions;
using Opus127.Dengue.Application.Models;
using Opus127.Dengue.Domain.ValueObjects;

namespace Opus127.Dengue.UnitTests.TestDoubles;

internal sealed class StubDengueDataSource(IReadOnlyCollection<DengueSourceRecord> records)
    : IDengueDataSource
{
    public EpidemiologicalWeek RequestedStart { get; private set; }

    public EpidemiologicalWeek RequestedEnd { get; private set; }

    public Task<IReadOnlyCollection<DengueSourceRecord>> FetchAsync(
        int geocode,
        EpidemiologicalWeek start,
        EpidemiologicalWeek end,
        CancellationToken cancellationToken)
    {
        RequestedStart = start;
        RequestedEnd = end;
        return Task.FromResult(records);
    }
}
