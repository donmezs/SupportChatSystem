using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SupportChatSystem.Application.BackgroundServices.Consumers.Interfaces;
using SupportChatSystem.Application.BackgroundServices.Models;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace SupportChatSystem.Application.BackgroundServices.Managers;
public class AgentQueueManager : IAgentQueueManager
{
    private readonly ILogger<AgentQueueManager> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnection _connection;
    private readonly string _agentQueueNamePattern;
    private readonly ConcurrentDictionary<Guid, IModel> _agentChannels;

    public AgentQueueManager(ILogger<AgentQueueManager> logger, IServiceProvider serviceProvider, IConnection connection, IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _connection = connection;
        _agentQueueNamePattern = configuration["RabbitMQ:AgentQueueNamePattern"];
        _agentChannels = new ConcurrentDictionary<Guid, IModel>();
    }

    public void EnsureListening(Guid agentId)
    {
        if (_agentChannels.ContainsKey(agentId))
        {
            _logger.LogInformation($"AgentQueueManager: Already listening to queue for agent {agentId}.");
            return;
        }

        var channel = _connection.CreateModel();
        var queueName = string.Format(_agentQueueNamePattern, agentId);
        channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var chatSessionMessage = JsonSerializer.Deserialize<ChatSessionMessage>(message);

            if (!chatSessionMessage.ChatSessionId.HasValue)
            {
                _logger.LogWarning("Received a message without a valid ChatSessionId.");
                channel.BasicAck(ea.DeliveryTag, false);
                return;
            }

            var chatSessionId = chatSessionMessage.ChatSessionId.Value;

            // Handle message
            using (var scope = _serviceProvider.CreateScope())
            {
                var agentQueueConsumer = scope.ServiceProvider.GetRequiredService<IAgentQueueConsumer>();
                agentQueueConsumer.ProcessMessageAsync(message, agentId, chatSessionId);
            }
        };

        channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

        _agentChannels.TryAdd(agentId, channel);

        _logger.LogInformation($"AgentQueueManager: Started listening to queue for agent {agentId}.");
    }

    public void PublishChatSessionToAgentQueue(Guid chatSessionId, Guid agentId)
    {
        var queueName = string.Format(_agentQueueNamePattern, agentId);
        var message = JsonSerializer.Serialize(new { ChatSessionId = chatSessionId });
        var body = Encoding.UTF8.GetBytes(message);

        if (_agentChannels.TryGetValue(agentId, out var channel))
        {
            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
            _logger.LogInformation($"Published chat session {chatSessionId} to {queueName}.");
        }
        else
        {
            _logger.LogWarning($"Attempted to publish to {queueName}, but the channel does not exist.");
        }
    }
}
