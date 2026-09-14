using Opus127.Dengue.Domain.ValueObjects;

namespace Opus127.Dengue.Application.Abstractions;

public interface IEpidemiologicalWeekCalculator
{
    EpidemiologicalWeek FromDate(DateOnly date);

    DateOnly GetStartDate(EpidemiologicalWeek week);
}
