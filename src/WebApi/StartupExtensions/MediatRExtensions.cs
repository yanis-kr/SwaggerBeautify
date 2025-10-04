using MediatR;

namespace WebApi.StartupExtensions;

public static class MediatRExtensions
{
    public static IServiceCollection AddMediatRSetup(this IServiceCollection services)
    {
        services.AddMediatR(typeof(Program).Assembly);
        return services;
    }
}
