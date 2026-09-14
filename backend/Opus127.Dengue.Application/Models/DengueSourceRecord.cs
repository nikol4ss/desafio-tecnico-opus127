namespace Opus127.Dengue.Application.Models;

public sealed record DengueSourceRecord(
    int EpidemiologicalYear,
    int EpidemiologicalWeek,
    DateOnly WeekStartDate,
    double EstimatedCases,
    double? EstimatedCasesMinimum,
    double? EstimatedCasesMaximum,
    int NotifiedCases,
    int AlertLevel,
    double? ProbabilityRtAboveOne,
    double? EstimatedIncidencePer100K,
    double? ReproductionNumber,
    long? SourceRecordId,
    string? ModelVersion);
