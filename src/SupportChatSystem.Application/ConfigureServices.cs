using Microsoft.Extensions.DependencyInjection;
using SupportChatSystem.Application.BackgroundServices.Consumers;
using SupportChatSystem.Application.BackgroundServices.Consumers.Interfaces;
using SupportChatSystem.Application.BackgroundServices.Managers;
using SupportChatSystem.Application.BackgroundServices.Services;
using SupportChatSystem.Application.Interfaces;
using SupportChatSystem.Application.Services;
using System.Reflection;

namespace SupportChatSystem.Application;
public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddScoped<IAgentManagementService, AgentManagementService>();
        services.AddScoped<IChatSessionManagementService, ChatSessionManagementService>();
        services.AddScoped<IShiftManagementService, ShiftManagementService>();
        services.AddScoped<ITeamManagementService, TeamManagementService>();

        services.AddSingleton<IAgentQueueManager, AgentQueueManager>();

        services.AddHostedService<ChatSessionQueueConsumer>();
        services.AddScoped<IAgentQueueConsumer, AgentQueueConsumer>();

        services.AddHostedService<PollingService>();

        return services;
    }
}

