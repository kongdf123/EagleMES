using EagleMES.Api.Data;
using EagleMES.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace EagleMES.Api.BackgroundServices
{
    public class DeviceSimulationService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<Hubs.DeviceHub> _hubContext;
        private readonly Random _random = new();

        public DeviceSimulationService(IServiceScopeFactory scopeFactory, IHubContext<DeviceHub> hubContext)
        {
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();

                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var devices = await context.Devices.ToListAsync(stoppingToken);

                foreach (var device in devices)
                {
                    device.Temperature = _random.Next(20, 100);
                    device.LastUpdated = DateTime.UtcNow;
                    if (device.Temperature > 40)
                    {
                        device.Status = "Warning";
                    }
                    else
                    {
                        device.Status = "Running";
                    }
                }

                await context.SaveChangesAsync(stoppingToken);

                // Notify clients about the update
                await _hubContext.Clients.All.SendAsync("ReceiveDeviceUpdate", devices, cancellationToken: stoppingToken);

                await Task.Delay(3000, stoppingToken); // Update every 3 seconds
            }
        }
    }
}
