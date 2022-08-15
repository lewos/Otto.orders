using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Otto.orders.DTOs;
using Otto.orders.Models;
using Otto.orders.Services;

namespace Otto.orders.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _orderService.GetAsync();
            return Ok(result);
        }

        [HttpGet("GetPendingOrders")]
        public async Task<IActionResult> GetPendingOrders()
        {
            var result = await _orderService.GetPendingAsync();
            return Ok(result);
        }


        [HttpGet("{id}", Name = "GetOrder")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var result = await _orderService.GetByIdAsync(id);
            return result != null ? (IActionResult)Ok(result) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Order order)
        {
            var result = await _orderService.CreateAsync(order);
            return Created("GetOrder", result.Item1);
        }

        [HttpPut("TakeOrderById/{id}")]
        public async Task<IActionResult> TakeOrder(int id, [FromBody] InProgressDTO dto)
        {
            dto.Id = id;
            var result = await _orderService.UpdateOrderInProgressAsync(id, dto.UserIdInProgress);
            if (result.Item2 > 0)
                return Ok(result.Item1);
            else
                return Conflict("No se encontro una orden con ese id o la misma ya se encuentra tomada por otro operario");
        }

        [HttpPut("StopTakingOrderById/{id}")]
        public async Task<IActionResult> StopTakingOrder(int id, [FromBody] InProgressDTO dto)
        {
            dto.Id = id;
            var result = await _orderService.UpdateOrderStopInProgressAsync(id, dto.UserIdInProgress);
            if (result.Item2 > 0)
                return Ok(result.Item1);
            else
                return Conflict("No se encontro una orden con ese id o el el id del operario no es el mismo que la tomo o la misma ya se encuentra en un estado final");
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Order order)
        {
            order.Id = id;
            var result = await _orderService.UpdateOrderTableAsync(id, order);
            return Ok(result.Item1);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            var result = await _orderService.DeleteAsync(id, order);
            return Ok(result);
        }
    }
}
