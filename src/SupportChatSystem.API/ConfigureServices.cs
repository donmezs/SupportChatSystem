using Microsoft.OpenApi.Models;

namespace SupportChatSystem.API;

public static class ConfigureServices
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllers();

        services.AddSwaggerGen(options =>
        {
            var version = "v1";

            options.SwaggerDoc(version, new OpenApiInfo
            {
                Title = $"Support Chat System",
                Version = version,
                Description = "Support Chat System"
            });
        });

        return services;
    }
}
