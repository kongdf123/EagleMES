using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using EagleMES.Api.DTOs;
using EagleMES.Api.Data;
using EagleMES.Api.Entities;

namespace EagleMES.Api.BackgroundServices
{
    public class WorkOrderCreatedConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        public WorkOrderCreatedConsumer(IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
        {
            IConnection? connection = null;

            var hostName = _configuration["RABBITMQ:HOSTNAME"] ?? "localhost";
            while (connection == null && !stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Console.WriteLine(
                        "Connecting to RabbitMQ...");

                    var factory = new ConnectionFactory
                    {
                        HostName = hostName,
                        RequestedConnectionTimeout = TimeSpan.FromSeconds(2)
                    };

                    connection =
                        await factory.CreateConnectionAsync(
                            cancellationToken: stoppingToken);

                    Console.WriteLine(
                        "Connected to RabbitMQ");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"RabbitMQ unavailable: {ex.Message}");

                    await Task.Delay(
                        5000,
                        stoppingToken);
                }
            }

            if (connection == null)
                return;

            var channel =
                await connection.CreateChannelAsync(
                    cancellationToken: stoppingToken);

            await channel.QueueDeclareAsync(
                queue: "workorder.created",
                durable: false,
                exclusive: false,
                autoDelete: false,
                cancellationToken: stoppingToken);

            var consumer =
                new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                var body = ea.Body.ToArray();

                var json =
                    Encoding.UTF8.GetString(body);

                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                db.EventLogs.Add(new EventLog
                {
                    EventType = "WorkOrderCreated",
                    Message = json,
                    CreatedAt = DateTime.UtcNow
                });

                await db.SaveChangesAsync(stoppingToken);

                var message =
                    JsonSerializer.Deserialize<WorkOrderCreatedEvent>(
                        json);

                Console.WriteLine(
                    $"Received event: {message!.ProductCode}");

                await Task.CompletedTask;
            };

            await channel.BasicConsumeAsync(
                queue: "workorder.created",
                autoAck: true,
                consumer: consumer,
                cancellationToken: stoppingToken);

            await Task.Delay(
                Timeout.Infinite,
                stoppingToken);
        }
    }
}
