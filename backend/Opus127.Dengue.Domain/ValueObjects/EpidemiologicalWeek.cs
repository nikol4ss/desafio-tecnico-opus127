namespace Opus127.Dengue.Domain.ValueObjects;

public readonly record struct EpidemiologicalWeek
{
    public EpidemiologicalWeek(int year, int week)
    {
        if (year is < 1900 or > 9999)
        {
            throw new ArgumentOutOfRangeException(nameof(year));
        }

        if (week is < 1 or > 53)
        {
            throw new ArgumentOutOfRangeException(nameof(week));
        }

        Year = year;
        Week = week;
    }

    public int Year { get; }

    public int Week { get; }

    public override string ToString() => $"{Year}-{Week:D2}";
}
