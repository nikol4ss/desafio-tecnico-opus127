using Opus127.Dengue.Application.Models;

namespace Opus127.Dengue.Application.Abstractions;

public interface IDengueSynchronizationService
{
    Task<DengueSyncResult> SynchronizeLastSixMonthsAsync(CancellationToken cancellationToken);
}
