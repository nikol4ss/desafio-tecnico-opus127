using System.Text.Json.Serialization;

namespace Opus127.Dengue.Infrastructure.ExternalServices;

internal sealed class AlertaDengueResponse
{
    [JsonPropertyName("data_iniSE")]
    public long WeekStartUnixMilliseconds { get; init; }

    [JsonPropertyName("SE")]
    public int EpidemiologicalWeekCode { get; init; }

    [JsonPropertyName("casos_est")]
    public double EstimatedCases { get; init; }

    [JsonPropertyName("casos_est_min")]
    public double? EstimatedCasesMinimum { get; init; }

    [JsonPropertyName("casos_est_max")]
    public double? EstimatedCasesMaximum { get; init; }

    [JsonPropertyName("casos")]
    public int NotifiedCases { get; init; }

    [JsonPropertyName("nivel")]
    public int AlertLevel { get; init; }

    [JsonPropertyName("p_rt1")]
    public double? ProbabilityRtAboveOne { get; init; }

    [JsonPropertyName("p_inc100k")]
    public double? EstimatedIncidencePer100K { get; init; }

    [JsonPropertyName("Rt")]
    public double? ReproductionNumber { get; init; }

    [JsonPropertyName("id")]
    public long? SourceRecordId { get; init; }

    [JsonPropertyName("versao_modelo")]
    public string? ModelVersion { get; init; }
}
