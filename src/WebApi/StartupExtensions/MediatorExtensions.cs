using System.Reflection;
using WebApi.Infrastructure.Mediator;

namespace WebApi.StartupExtensions;

/// <summary>
/// Extension methods for configuring the custom Mediator
/// </summary>
public static class MediatorExtensions
{
    /// <summary>
    /// Adds the custom Mediator and automatically registers all handlers from the specified assemblies
    /// </summary>
    public static IServiceCollection AddMediatorSetup(this IServiceCollection services)
    {
        // Register the mediator
        services.AddScoped<IMediator, Mediator>();

        // Auto-register all request handlers from the current assembly
        var assembly = Assembly.GetExecutingAssembly();
        RegisterHandlers(services, assembly);

        return services;
    }

    private static void RegisterHandlers(IServiceCollection services, Assembly assembly)
    {
        // Find all types that implement IRequestHandler<,>
        var handlerTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                .Select(i => new { Implementation = t, Interface = i }))
            .ToList();

        foreach (var handlerType in handlerTypes)
        {
            // Register as scoped (typical for request handlers)
            services.AddScoped(handlerType.Interface, handlerType.Implementation);
        }
    }
}

