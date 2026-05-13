using EagleMES.Api.Data;
using EagleMES.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EagleMES.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InventoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.Inventories
                .OrderBy(x => x.Id)
                .ToListAsync();

            return Ok(data);
        }

        [HttpPost("inbound")]
        public async Task<IActionResult> Inbound([FromBody] InventoryRequest request)
        {
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.MaterialCode == request.MaterialCode);

            if (inventory == null)
            {
                return NotFound();
            }

            inventory.Quantity += request.Quantity;

            await _context.SaveChangesAsync();
            return Ok(new
            {
                inventory,
                message = "Inventory added successfully"
            });
        }

        [HttpPost("outbound")]
        public async Task<IActionResult> Outbound([FromBody] InventoryRequest request)
        {
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.MaterialCode == request.MaterialCode);
            if (inventory == null)
            {
                return NotFound();
            }
            if (inventory.Quantity < request.Quantity)
            {
                return BadRequest(new
                {
                    message = "Not enough inventory"
                });
            }
            inventory.Quantity -= request.Quantity;
            if (inventory.Quantity < 0)
            {
                inventory.Quantity = 0;
            }

            await _context.SaveChangesAsync();
            return Ok(new
            {
                inventory,
                message = "Inventory removed successfully"
            });
        }

        // GET: api/<InventoryController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<InventoryController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<InventoryController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<InventoryController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<InventoryController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
