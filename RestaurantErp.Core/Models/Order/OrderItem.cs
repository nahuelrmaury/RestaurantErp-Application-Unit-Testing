using RestaurantErp.Core.Enums;

namespace RestaurantErp.Core.Models.Order
{
    public class OrderItem
    {
        public Guid ItemId { get; set; }

        public Guid ProductId { get; set; }

        public DateTime OrderingTime { get; set; }

        public int PersonId { get; set; }

        // Because price should be fixed in moment of ordering
        public decimal Price { get; set; }

        // Assumption:
        // we should keep information in our system about all cancelled items
        // for managing consumption of ingredients
        public bool IsCancelled { get; set; }
    }
}