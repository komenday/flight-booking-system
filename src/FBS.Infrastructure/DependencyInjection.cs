using FBS.Domain.Repositories;
using FBS.Domain.Services;
using FBS.Infrastructure.BackgroundJobs;
using FBS.Infrastructure.EventDispatcher;
using FBS.Infrastructure.Events;
using FBS.Infrastructure.Events.Mapping;
using FBS.Infrastructure.Events.Options;
using FBS.Infrastructure.Events.OptionsValidators;
using FBS.Infrastructure.Persistence;
using FBS.Infrastructure.Persistence.Repositories;
using FBS.Infrastructure.Seed;
using FBS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FBS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var defaultConnectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>((options) =>
        {
            options.UseSqlServer(
                defaultConnectionString,
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                    sqlOptions.CommandTimeout(30);
                }
            );

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

        services.Configure<EventPublisherOptions>(configuration.GetSection(EventPublisherOptions.SectionName));
        services.Configure<ServiceBusOptions>(configuration.GetSection(ServiceBusOptions.SectionName));

        services.AddSingleton<IValidateOptions<EventPublisherOptions>, EventPublisherOptionsValidator>();
        services.AddSingleton<IValidateOptions<ServiceBusOptions>, ServiceBusOptionsValidator>();

        services.AddSingleton<IEventMapper, EventMapper>();
        services.AddSingleton<IEventPublisher, ServiceBusEventPublisher>();

        services.AddScoped<ExpireReservationsJob>();
        services.AddScoped<FlightDataSeeder>();

        return services;
    }
}
