using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Opus127.Dengue.Application.Abstractions;
using Opus127.Dengue.Infrastructure.Configuration;
using Opus127.Dengue.Infrastructure.ExternalServices;
using Opus127.Dengue.Infrastructure.Persistence;
using Opus127.Dengue.Infrastructure.Persistence.Repositories;

namespace Opus127.Dengue.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DengueDatabase")
            ?? throw new InvalidOperationException(
                "Connection string 'DengueDatabase' was not configured.");

        services.AddDbContext<DengueDbContext>(options =>
            options.UseSqlServer(
                connectionString,
                sqlOptions => sqlOptions.EnableRetryOnFailure(3)));
        services.AddScoped<IDengueRepository, DengueRepository>();

        services
            .AddOptions<AlertaDengueOptions>()
            .Bind(configuration.GetSection(AlertaDengueOptions.SectionName))
            .Validate(options => Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _),
                "AlertaDengue:BaseUrl must be an absolute URL.")
            .Validate(options => options.TimeoutSeconds is > 0 and <= 120,
                "AlertaDengue:TimeoutSeconds must be between 1 and 120.")
            .ValidateOnStart();

        services.AddHttpClient<IDengueDataSource, AlertaDengueClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<AlertaDengueOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });

        return services;
    }
}
