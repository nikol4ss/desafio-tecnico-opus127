using System.Text.Json.Serialization;
using Opus127.Dengue.Application.Models;

namespace Opus127.Dengue.Api.Contracts;

public sealed record DengueWeekResponse(
    [property: JsonPropertyName("semana_epidemiologica")] string EpidemiologicalWeek,
    [property: JsonPropertyName("casos_est")] double EstimatedCases,
    [property: JsonPropertyName("casos_notificados")] int NotifiedCases,
    [property: JsonPropertyName("nivel_alerta")] int AlertLevel,
    [property: JsonPropertyName("data_inicio")] DateOnly WeekStartDate)
{
    public static DengueWeekResponse From(DengueWeekResult result) =>
        new(
            $"{result.EpidemiologicalYear}-{result.EpidemiologicalWeek:D2}",
            result.EstimatedCases,
            result.NotifiedCases,
            result.AlertLevel,
            result.WeekStartDate);
}
