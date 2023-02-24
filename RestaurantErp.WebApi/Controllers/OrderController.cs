using Microsoft.AspNetCore.Mvc;
using RestaurantErp.Core.Contracts;
using RestaurantErp.Core.Models.Order;
using System;
using System.Threading.Tasks;

namespace RestaurantErp.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderProvider _orderProvider;
        public OrderController(IOrderProvider orderProvider)
        {
            _orderProvider = orderProvider;
        }

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder()
        {
            var orderId = _orderProvider.CreateOrder();

            return new ObjectResult(orderId);
        }

        [HttpPost("AddItem")]
        public async Task<IActionResult> AddItem(OrderItemRequest request)
        {
            _orderProvider.AddItem(request);

            return StatusCode(200);
        }

        [HttpDelete("CancelItem")]
        public async Task<IActionResult> CancelItem(OrderItemRequest request)
        {
            _orderProvider.CancelItem(request);

            return StatusCode(200);
        }

        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout(Guid orderId)
        {
            var bill = _orderProvider.Checkout(orderId);

            return new ObjectResult(bill);
        }
    }
}
