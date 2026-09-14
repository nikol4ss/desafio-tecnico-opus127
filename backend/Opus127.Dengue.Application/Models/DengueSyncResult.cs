using Opus127.Dengue.Domain.ValueObjects;

namespace Opus127.Dengue.Application.Models;

public sealed record DengueSyncResult(
    EpidemiologicalWeek Start,
    EpidemiologicalWeek End,
    int ImportedRecords);
