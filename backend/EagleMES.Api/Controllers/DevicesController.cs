using EagleMES.Api.Data;
using EagleMES.Api.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace EagleMES.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevicesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<Hubs.DeviceHub> _hubContext;
        private readonly Random _random = new Random();

        public DevicesController(AppDbContext context, IHubContext<DeviceHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.Devices
                .ToListAsync();

            foreach (var device in data)
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

            return Ok(data);
        }

        [HttpPost("simulate")]
        public async Task<IActionResult> Simulate()
        {
            var devices = await _context.Devices.ToListAsync();
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
            await _context.SaveChangesAsync();

            // Broadcast to clients
            await _hubContext.Clients.All.SendAsync("ReceiveDeviceUpdate", devices);

            return Ok(devices);
        }
    }
}
