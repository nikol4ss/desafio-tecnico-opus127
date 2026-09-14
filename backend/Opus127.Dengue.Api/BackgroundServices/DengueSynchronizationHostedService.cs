using Microsoft.Extensions.Options;
using Opus127.Dengue.Application.Abstractions;
using Opus127.Dengue.Application.Configuration;

namespace Opus127.Dengue.Api.BackgroundServices;

public sealed class DengueSynchronizationHostedService(
    IServiceScopeFactory scopeFactory,
    IOptions<DengueDataOptions> options,
    ILogger<DengueSynchronizationHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!options.Value.SynchronizeOnStartup)
        {
            logger.LogInformation("Dengue data synchronization on startup is disabled.");
            return;
        }

        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var synchronizationService = scope.ServiceProvider
                .GetRequiredService<IDengueSynchronizationService>();
            var result = await synchronizationService
                .SynchronizeLastSixMonthsAsync(stoppingToken);

            logger.LogInformation(
                "Dengue data synchronized from {Start} through {End}. {Count} records imported.",
                result.Start,
                result.End,
                result.ImportedRecords);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Dengue data synchronization failed. The API will remain available with existing data.");
        }
    }
}
