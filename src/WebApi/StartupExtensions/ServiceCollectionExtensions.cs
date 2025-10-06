using Microsoft.AspNetCore.Mvc;

namespace WebApi.StartupExtensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            // Remove unwanted formatters to only keep JSON
            var xmlInputFormatter1 = options.InputFormatters.OfType<Microsoft.AspNetCore.Mvc.Formatters.XmlDataContractSerializerInputFormatter>().FirstOrDefault();
            if (xmlInputFormatter1 != null) options.InputFormatters.Remove(xmlInputFormatter1);
            
            var xmlInputFormatter2 = options.InputFormatters.OfType<Microsoft.AspNetCore.Mvc.Formatters.XmlSerializerInputFormatter>().FirstOrDefault();
            if (xmlInputFormatter2 != null) options.InputFormatters.Remove(xmlInputFormatter2);
            
            var xmlOutputFormatter1 = options.OutputFormatters.OfType<Microsoft.AspNetCore.Mvc.Formatters.XmlDataContractSerializerOutputFormatter>().FirstOrDefault();
            if (xmlOutputFormatter1 != null) options.OutputFormatters.Remove(xmlOutputFormatter1);
            
            var xmlOutputFormatter2 = options.OutputFormatters.OfType<Microsoft.AspNetCore.Mvc.Formatters.XmlSerializerOutputFormatter>().FirstOrDefault();
            if (xmlOutputFormatter2 != null) options.OutputFormatters.Remove(xmlOutputFormatter2);
        });
        services.AddEndpointsApiExplorer();
        
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        return services;
    }
}
