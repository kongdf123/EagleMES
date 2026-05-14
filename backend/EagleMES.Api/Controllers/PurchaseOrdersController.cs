using EagleMES.Api.Data;
using EagleMES.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EagleMES.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PurchaseOrdersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.PurchaseOrders.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create(PurchaseOrder po)
        {
            po.Status = "Created";
            _context.PurchaseOrders.Add(po);
            await _context.SaveChangesAsync();
            return Ok(po);
        }
    }
}
