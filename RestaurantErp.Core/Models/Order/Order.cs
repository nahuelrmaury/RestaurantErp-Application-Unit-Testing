using System.Collections.Concurrent;

namespace RestaurantErp.Core
{
    public class Order
    {
        public Guid Id { get; set; }

        //public bool IsPayed { get; set; }
    
        public List<OrderItem> Items { get; set; }
    }

    public class OrderItem
    {
        public DishEnum Dish { get; set; }

        public int PersonId { get; set; }
    }

    public class Bill
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public IEnumerable<BillItem> Items { get; set; }

        public decimal Amount { get; set; }

        public decimal Discount { get; set; }

        public decimal AmountDiscounted { get; set; }

        // TODO:
        // move to separated class 
        public void ApplyDiscount(BillDiscountInfo billDiscountInfo)
        {
            Discount += billDiscountInfo.Discount;
            AmountDiscounted -= billDiscountInfo.Discount;

            foreach (var itemInfo in billDiscountInfo.Items)
            {
                var targetItem = Items.Single(i => i.ItemId == itemInfo.ItemId);

                targetItem.Discount += itemInfo.Discount;
                targetItem.AmountDiscounted -= itemInfo.Discount;
            }
        }
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