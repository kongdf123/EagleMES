using EagleMES.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EagleMES.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DevicesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly Random _random = new Random();

        public DevicesController(AppDbContext context)
        {
            _context = context;
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
    }
}
