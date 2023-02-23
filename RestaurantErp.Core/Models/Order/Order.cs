using System.Collections.Concurrent;

namespace RestaurantErp.Core.Models.Order
{
    public class Order
    {
        public Guid Id { get; set; }

        //public bool IsPayed { get; set; }

        public ConcurrentBag<OrderItem> Items { get; set; } = new ConcurrentBag<OrderItem>();
    }
}