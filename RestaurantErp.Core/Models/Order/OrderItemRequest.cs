using RestaurantErp.Core.Enums;

namespace RestaurantErp.Core.Models.Order
{
    public class OrderItemRequest
    {
        public Guid OrderId { get; set; }

        public int GuestNumber { get; set; }

        public Guid ProductId { get; set; }

        public int Count { get; set; }
    }
}