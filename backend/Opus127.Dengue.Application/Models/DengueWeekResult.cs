namespace Opus127.Dengue.Application.Models;

public sealed record DengueWeekResult(
    int EpidemiologicalYear,
    int EpidemiologicalWeek,
    DateOnly WeekStartDate,
    double EstimatedCases,
    int NotifiedCases,
    int AlertLevel);
