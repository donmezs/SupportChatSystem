using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using SupportChatSystem.Infrastructure.Data;

namespace SupportChatSystem.Infrastructure;
public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMQSettings = configuration.GetSection("RabbitMQ");
        services.AddSingleton<IConnection>(sp =>
        {
            var factory = new ConnectionFactory()
            {
                HostName = rabbitMQSettings["HostName"],
                UserName = rabbitMQSettings["UserName"],
                Password = rabbitMQSettings["Password"]
            };
            return factory.CreateConnection();
        });

        services.AddSingleton<IModel>(sp =>
        {
            var connection = sp.GetRequiredService<IConnection>();
            return connection.CreateModel();
        });

        DbExtensions.AddRepositories(services);
        DbExtensions.AddDbContext(services, configuration);

        return services;
    }
}
