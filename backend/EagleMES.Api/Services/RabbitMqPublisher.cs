using EagleMES.Api.DTOs;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace EagleMES.Api.Services
{
    public class RabbitMqPublisher
    {
        private readonly IConfiguration _configuration;

        public RabbitMqPublisher(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task PublishWorkOrderEventAsync(WorkOrderCreatedEvent @event)
        {
            var hostName = _configuration["RABBITMQ:HOSTNAME"] ?? "localhost";

            var factory = new ConnectionFactory
            {
                HostName = hostName,
                RequestedConnectionTimeout = TimeSpan.FromSeconds(2)
            };

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "workorder.created",
                durable: false,
                exclusive: false,
                autoDelete: false);

            var body = Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(@event));

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "workorder.created",
                body: body);
        }
    }
}
