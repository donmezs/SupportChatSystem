using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SupportChatSystem.Application.BackgroundServices.Consumers.Interfaces;
using SupportChatSystem.Application.BackgroundServices.Models;
using SupportChatSystem.Application.Interfaces;
using System.Text;
using System.Text.Json;

namespace SupportChatSystem.Application.BackgroundServices.Consumers;
public class AgentQueueConsumer : BackgroundService, IAgentQueueConsumer
{
    private readonly ILogger<AgentQueueConsumer> _logger;
    private readonly IServiceProvider _serviceProvider;
    private IConnection _connection;
    private IModel _channel;
    private readonly string _hostname;
    private readonly string _agentQueueNamePattern;

    public AgentQueueConsumer(ILogger<AgentQueueConsumer> logger,
                              IServiceProvider serviceProvider,
                              IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _hostname = configuration["RabbitMQ:Hostname"];
        _agentQueueNamePattern = configuration["RabbitMQ:AgentQueueNamePattern"];
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var factory = new ConnectionFactory { HostName = _hostname };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        return Task.CompletedTask;
    }

    public void StartListening(Guid agentId)
    {
        var queueName = string.Format(_agentQueueNamePattern, agentId);
        _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var chatSessionMessage = JsonSerializer.Deserialize<ChatSessionMessage>(message);

            if (!chatSessionMessage.ChatSessionId.HasValue)
            {
                _logger.LogWarning("Received a message without a valid ChatSessionId.");
                _channel.BasicAck(ea.DeliveryTag, false);
                return;
            }

            var chatSessionId = chatSessionMessage.ChatSessionId.Value;

            _logger.LogInformation($"Received message for agent {agentId}: {message}");

            await ProcessMessageAsync(message, agentId, chatSessionId);

            _channel.BasicAck(ea.DeliveryTag, false);
        };

        _logger.LogInformation($"Started listening to the queue '{queueName}' for agent {agentId}.");
    }

    public async Task ProcessMessageAsync(string message, Guid agentId, Guid chatSessionId)
    {
        using (var scope = _serviceProvider.CreateScope()) // Create a scope
        {
            var chatSessionManagementService = scope.ServiceProvider.GetRequiredService<IChatSessionManagementService>();
            try
            {
                var result = await chatSessionManagementService.SetChatSessionStatus(chatSessionId, Domain.Enums.ChatSessionStatus.Active);
                if (result)
                {
                    _logger.LogInformation("Chat session {0} is active now.", chatSessionId);
                }
                else
                {
                    _logger.LogWarning("Chat session {0} couldn't activated.", chatSessionId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat session {0}.", chatSessionId);
            }
        }

    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}