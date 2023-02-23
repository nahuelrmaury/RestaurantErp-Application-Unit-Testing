using Microsoft.AspNetCore.Mvc;
using RestaurantErp.Core.Contracts;
using RestaurantErp.Core.Models.Discount;
using RestaurantErp.Core.Models.Product;
using System;
using System.Threading.Tasks;

namespace RestaurantErp.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        public readonly IProductProvider _productProvider;
        public readonly IDiscountByTimeProvider _discountByTimeProvider;

        public AdminController(IProductProvider productProvider,
            IDiscountByTimeProvider discountByTimeProvider)
        {
            _productProvider = productProvider;
            _discountByTimeProvider = discountByTimeProvider;
        }

        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct(AddProductRequest productInfo)
        {
            var orderId = _productProvider.AddProduct(productInfo);

            return new ObjectResult(orderId);
        }

        [HttpPost("AddDiscountByTimeSettings")]
        public async Task<IActionResult> AddDiscountByTimeSettings(DiscountByTimeSettings settings)
        {
            var discountId = _discountByTimeProvider.Add(settings);

            return new ObjectResult(discountId);
        }

        [HttpDelete("RemoveDiscountByTimeSettings")]
        public async Task<IActionResult> removeDiscountByTimeSettings(Guid id)
        {
            var discountId = _discountByTimeProvider.Remove(id);

            return new ObjectResult(discountId);
        }
    }
}
