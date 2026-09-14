using Opus127.Dengue.Api.BackgroundServices;
using Opus127.Dengue.Application.Abstractions;
using Opus127.Dengue.Application.Configuration;
using Opus127.Dengue.Application.Services;
using Opus127.Dengue.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

builder.Services
    .AddOptions<DengueDataOptions>()
    .Bind(builder.Configuration.GetSection(DengueDataOptions.SectionName))
    .Validate(options => options.Geocode > 0, "DengueData:Geocode must be positive.")
    .ValidateOnStart();

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IEpidemiologicalWeekCalculator, EpidemiologicalWeekCalculator>();
builder.Services.AddScoped<IDengueQueryService, DengueQueryService>();
builder.Services.AddScoped<IDengueSynchronizationService, DengueSynchronizationService>();
builder.Services.AddHostedService<DengueSynchronizationHostedService>();
builder.Services.AddInfrastructure(builder.Configuration);

var allowedOrigins = builder.Configuration
    .GetSection("AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
        }
    }));

var app = builder.Build();

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
    .ExcludeFromDescription();

await app.Services.InitializeDatabaseAsync();
await app.RunAsync();

public partial class Program;
