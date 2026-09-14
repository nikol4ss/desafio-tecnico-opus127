using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Opus127.Dengue.Application.Abstractions;
using Opus127.Dengue.Application.Models;
using Opus127.Dengue.Domain.ValueObjects;
using Opus127.Dengue.Infrastructure.Configuration;

namespace Opus127.Dengue.Infrastructure.ExternalServices;

public sealed class AlertaDengueClient(
    HttpClient httpClient,
    IOptions<AlertaDengueOptions> options) : IDengueDataSource
{
    private readonly AlertaDengueOptions _options = options.Value;

    public async Task<IReadOnlyCollection<DengueSourceRecord>> FetchAsync(
        int geocode,
        EpidemiologicalWeek start,
        EpidemiologicalWeek end,
        CancellationToken cancellationToken)
    {
        var requestUri =
            $"api/alertcity?geocode={geocode}" +
            $"&disease={Uri.EscapeDataString(_options.Disease)}" +
            "&format=json" +
            $"&ew_start={start.Week}&ew_end={end.Week}" +
            $"&ey_start={start.Year}&ey_end={end.Year}";

        try
        {
            using var response = await httpClient.GetAsync(requestUri, cancellationToken);
            response.EnsureSuccessStatusCode();

            var sourceRecords = await response.Content.ReadFromJsonAsync<List<AlertaDengueResponse>>(
                cancellationToken: cancellationToken) ?? [];

            return sourceRecords
                .Where(IsValid)
                .Select(Map)
                .GroupBy(record => (record.EpidemiologicalYear, record.EpidemiologicalWeek))
                .Select(group => group.OrderByDescending(record => record.ModelVersion).First())
                .OrderBy(record => record.EpidemiologicalYear)
                .ThenBy(record => record.EpidemiologicalWeek)
                .ToArray();
        }
        catch (Exception exception) when (exception is HttpRequestException or JsonException)
        {
            throw new InvalidOperationException(
                "Unable to retrieve dengue data from the AlertaDengue API.",
                exception);
        }
    }

    private static bool IsValid(AlertaDengueResponse source) =>
        source.EpidemiologicalWeekCode is >= 190001 and <= 999953 &&
        source.WeekStartUnixMilliseconds > 0 &&
        source.EstimatedCases >= 0 &&
        source.NotifiedCases >= 0 &&
        source.AlertLevel is >= 1 and <= 4;

    private static DengueSourceRecord Map(AlertaDengueResponse source)
    {
        var year = source.EpidemiologicalWeekCode / 100;
        var week = source.EpidemiologicalWeekCode % 100;
        var weekStartDate = DateOnly.FromDateTime(
            DateTimeOffset
                .FromUnixTimeMilliseconds(source.WeekStartUnixMilliseconds)
                .UtcDateTime);

        return new DengueSourceRecord(
            year,
            week,
            weekStartDate,
            source.EstimatedCases,
            source.EstimatedCasesMinimum,
            source.EstimatedCasesMaximum,
            source.NotifiedCases,
            source.AlertLevel,
            source.ProbabilityRtAboveOne,
            source.EstimatedIncidencePer100K,
            source.ReproductionNumber,
            source.SourceRecordId,
            source.ModelVersion);
    }
}
