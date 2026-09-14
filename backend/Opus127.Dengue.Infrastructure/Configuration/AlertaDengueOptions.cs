namespace Opus127.Dengue.Infrastructure.Configuration;

public sealed class AlertaDengueOptions
{
    public const string SectionName = "AlertaDengue";

    public string BaseUrl { get; set; } = "https://info.dengue.mat.br/";

    public string Disease { get; set; } = "dengue";

    public int TimeoutSeconds { get; set; } = 30;
}
