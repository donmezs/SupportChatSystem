using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SupportChatSystem.Application.BackgroundServices.Managers;
using SupportChatSystem.Application.BackgroundServices.Models;
using SupportChatSystem.Application.Interfaces;
using System.Text;
using System.Text.Json;

namespace SupportChatSystem.Application.BackgroundServices.Consumers;
public class ChatSessionQueueConsumer : BackgroundService
{
    private readonly ILogger<ChatSessionQueueConsumer> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IAgentQueueManager _agentQueueManager;
    private readonly string _hostname;
    private readonly string _queueName;
    private IConnection _connection;
    private IModel _channel;

    public ChatSessionQueueConsumer(ILogger<ChatSessionQueueConsumer> logger,
                                    IServiceProvider serviceProvider,
                                    IAgentQueueManager agentQueueManager,
                                    IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _agentQueueManager = agentQueueManager;
        _hostname = configuration["RabbitMQ:Hostname"];
        _queueName = configuration["RabbitMQ:ChatSessionQueueName"];
        InitializeRabbitMQ();
    }

    private void InitializeRabbitMQ()
    {
        var factory = new ConnectionFactory { HostName = _hostname };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: true, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

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

            _logger.LogInformation("Received chat session creation message: {0}", chatSessionId);

            using (var scope = _serviceProvider.CreateScope()) // Create a scope
            {
                var chatSessionManagementService = scope.ServiceProvider.GetRequiredService<IChatSessionManagementService>();
                try
                {
                    var (IsAssigned, AssignedAgentId) = await chatSessionManagementService.AssignChatSessionAsync(chatSessionId);
                    if (IsAssigned && AssignedAgentId.HasValue)
                    {
                        _logger.LogInformation("Chat session {0} assigned to agent {1}.", chatSessionId, AssignedAgentId.Value);
                        PublishChatSessionToAgentQueue(chatSessionId, AssignedAgentId.Value);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to assign chat session {0}.", chatSessionId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing chat session {0}.", chatSessionId);
                }
            }

            _channel.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }

    private void PublishChatSessionToAgentQueue(Guid chatSessionId, Guid agentId)
    {
        _agentQueueManager.EnsureListening(agentId);
        _agentQueueManager.PublishChatSessionToAgentQueue(chatSessionId, agentId);
        _logger.LogInformation($"Chat session {chatSessionId} published to queue for agent {agentId}.");
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}