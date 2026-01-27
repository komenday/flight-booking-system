using Microsoft.Extensions.DependencyInjection;

namespace FBS.Application;

public static class DependencyInjection
{
    // TODO
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        //var assembly = Assembly.GetExecutingAssembly();

        //// MediatR
        //services.AddMediatR(cfg =>
        //{
        //    cfg.RegisterServicesFromAssembly(assembly);

        //    // Pipeline behaviors (order matters!)
        //    //cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        //    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        //    //cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
        //});

        //// FluentValidation
        //services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
