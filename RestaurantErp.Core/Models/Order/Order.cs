using System.Collections.Concurrent;

namespace RestaurantErp.Core
{
    public class Order
    {
        public Guid Id { get; set; }

        //public bool IsPayed { get; set; }
    
        public ConcurrentBag<OrderItem> Items { get; set; } = new ConcurrentBag<OrderItem>();
    }

    public class OrderItem
    {
        public Guid ItemId { get; set; }

        public DishEnum Dish { get; set; }

        public DateTime OrderingTime { get; set; }

        public int PersonId { get; set; }

        // Because price should be fixed in moment of ordering
        public decimal Price { get; set; }

        // Assumption:
        // we should keep information in our system about all cancelled items
        // for managing consumption of ingredients
        public bool IsCancelled { get; set; }
    }

    public class Bill
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public IEnumerable<BillItem> Items { get; set; }

        public decimal Amount { get; set; }

        public decimal Discount { get; set; }

        public decimal AmountDiscounted { get; set; }
    }

    public class BillItem
    {   
        public Guid ItemId { get; set; }

        public DishEnum Dish { get; set; }

        public int PersonId { get; set; }

        public decimal Amount { get; set; }

        public decimal Discount { get; set; }

        public decimal AmountDiscounted { get; set; }
    }
}