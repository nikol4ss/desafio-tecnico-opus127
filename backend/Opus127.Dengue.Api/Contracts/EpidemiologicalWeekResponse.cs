using System.Text.Json.Serialization;
using Opus127.Dengue.Domain.ValueObjects;

namespace Opus127.Dengue.Api.Contracts;

public sealed record EpidemiologicalWeekResponse(
    [property: JsonPropertyName("ey")] int Year,
    [property: JsonPropertyName("ew")] int Week,
    [property: JsonPropertyName("semana_epidemiologica")] string Label)
{
    public static EpidemiologicalWeekResponse From(EpidemiologicalWeek week) =>
        new(week.Year, week.Week, week.ToString());
}
