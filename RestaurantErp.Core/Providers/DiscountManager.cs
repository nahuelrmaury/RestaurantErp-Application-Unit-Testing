namespace RestaurantErp.Core
{
    public class DiscountByTimeManager : IDiscountManager
    {
        private List<Action<Order>> _actions = new List<Action<Order>>();

        public void Add(Action<Order> action)
        {
            throw new NotImplementedException();
        }

        public BillDiscountInfo Calculate(Order order)
        {
            throw new NotImplementedException();
        }
    }

    public interface IDiscountManager
    {
        BillDiscountInfo Calculate(Order order);   

        void Add(Action<Order> action);   
    }

    public class BillDiscountInfo
    {
        public decimal Discount { get; set; }

        public IEnumerable<BillDiscountItemInfo> Items { get; set; }
    }

    public class BillDiscountItemInfo
    {
        public Guid ItemId { get; set; }

        public decimal Discount { get; set; }
    }
}
