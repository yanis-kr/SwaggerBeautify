namespace WebApi.StartupExtensions;

public static class ValidationExtensions
{
    public static IServiceCollection AddValidationSetup(this IServiceCollection services)
    {
        // TODO: Add FluentValidation when package is installed
        // services.AddValidatorsFromAssemblyContaining<Program>();
        
        return services;
    }
}
