using EagleMES.Api.Data;
using EagleMES.Api.DTOs;
using EagleMES.Api.Entities;
using EagleMES.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EagleMES.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkOrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly RabbitMqPublisher _publisher; 
        private readonly ILogger<WorkOrdersController> _logger;

        public WorkOrdersController(AppDbContext context,RabbitMqPublisher publisher, ILogger<WorkOrdersController> logger)
        {
            _context = context;
            _publisher = publisher;
            _logger = logger;
        }

        // GET: api/workorders
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<WorkOrder>>> GetAll()
        {
            var workOrders = await _context.WorkOrders
                .OrderByDescending(w => w.Id)
                .ToListAsync();

            return Ok(workOrders);
        }

        // GET: api/workorders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkOrder>> GetById(int id)
        {
            var workOrder = await _context.WorkOrders.FindAsync(id);

            if (workOrder == null)
            {
                return NotFound(new
                {
                    message = $"WorkOrder {id} not found"
                });
            }

            return Ok(workOrder);
        }

        // GET: api/<WorkOrdersController>
        //[HttpGet]
        //public async Task<IActionResult> Get()
        //{
        //    var data = await _context.WorkOrders.ToListAsync();
        //    return Ok(data);
        //}
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<WorkOrdersController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST: api/workorders
        //{
        //  "orderNo": "WO-1003",
        //  "productCode": "P-CX03",
        //  "quantity": 300,
        //  "status": "Pending"
        //}
        [HttpPost]
        public async Task<ActionResult<WorkOrder>> Create(CreateWorkOrderRequest request)
        {
            // 简单校验
            if (string.IsNullOrWhiteSpace(request.OrderNo))
            {
                return BadRequest(new
                {
                    message = "OrderNo is required"
                });
            }

            if (string.IsNullOrWhiteSpace(request.ProductCode))
            {
                return BadRequest(new
                {
                    message = "ProductCode is required"
                });
            }

            var workOrder = new WorkOrder
            {
                OrderNo = request.OrderNo,
                ProductCode = request.ProductCode,
                Quantity = request.Quantity,
                Status = "Pending",
                StartTime = DateTime.UtcNow
            }; 

            _context.WorkOrders.Add(workOrder);
            await _context.SaveChangesAsync();
            try
            {
                await _publisher.PublishWorkOrderEventAsync(new WorkOrderCreatedEvent
                {
                    Id = workOrder.Id,
                    OrderNo = workOrder.OrderNo,
                    ProductCode = request.ProductCode,
                    Quantity = request.Quantity,
                    CreatedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "RabbitMQ publish failed for work order {OrderNo}",
                    workOrder.OrderNo);
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = workOrder.Id },
                workOrder
            );
        }
        // POST api/<WorkOrdersController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        // PUT api/<WorkOrdersController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<WorkOrdersController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
