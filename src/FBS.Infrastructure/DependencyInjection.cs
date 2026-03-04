using FBS.Domain.Repositories;
using FBS.Domain.Services;
using FBS.Infrastructure.BackgroundJobs;
using FBS.Infrastructure.EventDispatcher;
using FBS.Infrastructure.Events;
using FBS.Infrastructure.OptionsValidators;
using FBS.Infrastructure.Persistence;
using FBS.Infrastructure.Persistence.Repositories;
using FBS.Infrastructure.Seed;
using FBS.Infrastructure.Services;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Retry;

namespace FBS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);

                    sqlOptions.CommandTimeout(30);
                });

            // Only for Development
            var enableSensitiveLogging = configuration["EnableSensitiveDataLogging"];
            if (bool.TryParse(enableSensitiveLogging, out var isEnabled) && isEnabled)
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        services.AddScoped(serviceProvider =>
            new Lazy<IDomainEventDispatcher>(() =>
                serviceProvider.GetRequiredService<IDomainEventDispatcher>()));

        services.AddScoped<IFlightRepository, FlightRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IExecutionStrategy, EfCoreExecutionStrategy>();

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IEventPublisher, HttpEventPublisher>();

        services.Configure<EventPublisherOptions>(
            configuration.GetSection(EventPublisherOptions.SectionName));

        services.AddSingleton<IValidateOptions<EventPublisherOptions>, EventPublisherOptionsValidator>();

        services.AddHttpClient<NotificationSystemHttpClient>((serviceProvider, httpClient) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<EventPublisherOptions>>().Value;

            httpClient.BaseAddress = new Uri(options.BaseUrl);
            httpClient.DefaultRequestHeaders.Add("X-API-Key", options.ApiKey);
            httpClient.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        })
        .AddPolicyHandler(GetCircuitBreakerPolicy())
        .AddPolicyHandler(GetRetryPolicy());

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(
                configuration.GetConnectionString("DefaultConnection"),
                new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true,
                    SchemaName = "Hangfire"
                }));

        services.AddHangfireServer(options =>
        {
            options.WorkerCount = 2;
            options.ServerName = "FBS-BackgroundProcessor";
        });

        services.AddScoped<ExpireReservationsJob>();
        services.AddScoped<FlightDataSeeder>();

        return services;
    }

    private static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
            );
    }

    private static AsyncCircuitBreakerPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .AdvancedCircuitBreakerAsync(
                failureThreshold: 0.5,
                samplingDuration: TimeSpan.FromSeconds(60),
                minimumThroughput: 5,
                durationOfBreak: TimeSpan.FromSeconds(5)
            );
    }
}
