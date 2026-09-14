namespace Opus127.Dengue.Domain.Entities;

public sealed class DengueRecord
{
    private DengueRecord()
    {
    }

    private DengueRecord(
        int geocode,
        int epidemiologicalYear,
        int epidemiologicalWeek,
        DateOnly weekStartDate,
        double estimatedCases,
        double? estimatedCasesMinimum,
        double? estimatedCasesMaximum,
        int notifiedCases,
        int alertLevel,
        double? probabilityRtAboveOne,
        double? estimatedIncidencePer100K,
        double? reproductionNumber,
        long? sourceRecordId,
        string? modelVersion,
        DateTimeOffset syncedAtUtc)
    {
        Geocode = geocode;
        EpidemiologicalYear = epidemiologicalYear;
        EpidemiologicalWeek = epidemiologicalWeek;
        ApplyData(
            weekStartDate,
            estimatedCases,
            estimatedCasesMinimum,
            estimatedCasesMaximum,
            notifiedCases,
            alertLevel,
            probabilityRtAboveOne,
            estimatedIncidencePer100K,
            reproductionNumber,
            sourceRecordId,
            modelVersion,
            syncedAtUtc);
    }

    public long Id { get; private set; }

    public int Geocode { get; private set; }

    public int EpidemiologicalYear { get; private set; }

    public int EpidemiologicalWeek { get; private set; }

    public DateOnly WeekStartDate { get; private set; }

    public double EstimatedCases { get; private set; }

    public double? EstimatedCasesMinimum { get; private set; }

    public double? EstimatedCasesMaximum { get; private set; }

    public int NotifiedCases { get; private set; }

    public int AlertLevel { get; private set; }

    public double? ProbabilityRtAboveOne { get; private set; }

    public double? EstimatedIncidencePer100K { get; private set; }

    public double? ReproductionNumber { get; private set; }

    public long? SourceRecordId { get; private set; }

    public string? ModelVersion { get; private set; }

    public DateTimeOffset SyncedAtUtc { get; private set; }

    public static DengueRecord Create(
        int geocode,
        int epidemiologicalYear,
        int epidemiologicalWeek,
        DateOnly weekStartDate,
        double estimatedCases,
        double? estimatedCasesMinimum,
        double? estimatedCasesMaximum,
        int notifiedCases,
        int alertLevel,
        double? probabilityRtAboveOne,
        double? estimatedIncidencePer100K,
        double? reproductionNumber,
        long? sourceRecordId,
        string? modelVersion,
        DateTimeOffset syncedAtUtc) =>
        new(
            geocode,
            epidemiologicalYear,
            epidemiologicalWeek,
            weekStartDate,
            estimatedCases,
            estimatedCasesMinimum,
            estimatedCasesMaximum,
            notifiedCases,
            alertLevel,
            probabilityRtAboveOne,
            estimatedIncidencePer100K,
            reproductionNumber,
            sourceRecordId,
            modelVersion,
            syncedAtUtc);

    public void UpdateFrom(DengueRecord source)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (Geocode != source.Geocode ||
            EpidemiologicalYear != source.EpidemiologicalYear ||
            EpidemiologicalWeek != source.EpidemiologicalWeek)
        {
            throw new InvalidOperationException("The epidemiological record identity cannot be changed.");
        }

        ApplyData(
            source.WeekStartDate,
            source.EstimatedCases,
            source.EstimatedCasesMinimum,
            source.EstimatedCasesMaximum,
            source.NotifiedCases,
            source.AlertLevel,
            source.ProbabilityRtAboveOne,
            source.EstimatedIncidencePer100K,
            source.ReproductionNumber,
            source.SourceRecordId,
            source.ModelVersion,
            source.SyncedAtUtc);
    }

    private void ApplyData(
        DateOnly weekStartDate,
        double estimatedCases,
        double? estimatedCasesMinimum,
        double? estimatedCasesMaximum,
        int notifiedCases,
        int alertLevel,
        double? probabilityRtAboveOne,
        double? estimatedIncidencePer100K,
        double? reproductionNumber,
        long? sourceRecordId,
        string? modelVersion,
        DateTimeOffset syncedAtUtc)
    {
        if (estimatedCases < 0 || notifiedCases < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(estimatedCases), "Case totals cannot be negative.");
        }

        if (alertLevel is < 1 or > 4)
        {
            throw new ArgumentOutOfRangeException(nameof(alertLevel));
        }

        WeekStartDate = weekStartDate;
        EstimatedCases = estimatedCases;
        EstimatedCasesMinimum = estimatedCasesMinimum;
        EstimatedCasesMaximum = estimatedCasesMaximum;
        NotifiedCases = notifiedCases;
        AlertLevel = alertLevel;
        ProbabilityRtAboveOne = probabilityRtAboveOne;
        EstimatedIncidencePer100K = estimatedIncidencePer100K;
        ReproductionNumber = reproductionNumber;
        SourceRecordId = sourceRecordId;
        ModelVersion = string.IsNullOrWhiteSpace(modelVersion) ? null : modelVersion.Trim();
        SyncedAtUtc = syncedAtUtc;
    }
}
