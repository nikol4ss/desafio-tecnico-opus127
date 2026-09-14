using System.Globalization;
using Opus127.Dengue.Application.Abstractions;
using Opus127.Dengue.Domain.ValueObjects;

namespace Opus127.Dengue.Application.Services;

public sealed class EpidemiologicalWeekCalculator : IEpidemiologicalWeekCalculator
{
    public EpidemiologicalWeek FromDate(DateOnly date)
    {
        var shiftedDate = date.ToDateTime(TimeOnly.MinValue).AddDays(1);

        return new EpidemiologicalWeek(
            ISOWeek.GetYear(shiftedDate),
            ISOWeek.GetWeekOfYear(shiftedDate));
    }

    public DateOnly GetStartDate(EpidemiologicalWeek week)
    {
        var isoMonday = ISOWeek.ToDateTime(week.Year, week.Week, DayOfWeek.Monday);
        return DateOnly.FromDateTime(isoMonday.AddDays(-1));
    }
}
