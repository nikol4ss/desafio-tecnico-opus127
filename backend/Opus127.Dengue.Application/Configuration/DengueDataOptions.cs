namespace Opus127.Dengue.Application.Configuration;

public sealed class DengueDataOptions
{
    public const string SectionName = "DengueData";

    public int Geocode { get; set; } = 3106200;

    public bool SynchronizeOnStartup { get; set; } = true;
}
