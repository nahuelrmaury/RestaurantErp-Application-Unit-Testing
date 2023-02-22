using System.Collections.Concurrent;

namespace RestaurantErp.Core
{
    public class OrderProvider: IOrderProvider
    {
        private ConcurrentBag<Order> _orderStorage = new ConcurrentBag<Order>();

        private IPriceStorage _priceStorage;
        private IEnumerable<IDiscountManager> _discountManagers;

        public OrderProvider(IPriceStorage priceStorage,
            IEnumerable<IDiscountManager> discountManagers)
        {
            _priceStorage = priceStorage;
            _discountManagers = discountManagers;
        }

        public Guid CreateOrder()
        {
            var order = new Order();

            order.Id = Guid.NewGuid();
            _orderStorage.Add(order);

            return order.Id;
        }

        public void AddItem(OrderItemRequest request)
        {
            var targetOrder = _orderStorage.Single(i => i.Id == request.OrderId);
        }

        public void CancelItem(OrderItemRequest request)
        {
            throw new NotImplementedException();
        }


        // TODO:
        // add calback logic.
        // after changing order (AddItem / CancelItem methods) - previous bill must be compromised
        public Bill Checkout(Guid orderId)
        {
            var targetOrder = _orderStorage.Single(i => i.Id == orderId);

            var billItems = targetOrder.Items.Select(i => new BillItem
            {
                Dish = i.Dish,
                PersonId = i.PersonId,
                Amount = _priceStorage.GetProductPrice(i.Dish),
                AmountDiscounted = _priceStorage.GetProductPrice(i.Dish)
            });

            var bill = new Bill
            {
                OrderId = orderId,
                Items = billItems,
                Amount = billItems.Sum(i => i.Amount),
                AmountDiscounted = billItems.Sum(i => i.AmountDiscounted)
            };

            var discounts = _discountManagers.Select(i => i.Calculate(targetOrder));

            foreach(var discountInfo in discounts)
            {
                bill.ApplyDiscount(discountInfo);
            }

            return bill;
        }
    }

    public interface IOrderProvider
    {
        Guid CreateOrder();

        void AddItem(OrderItemRequest request);

        void CancelItem(OrderItemRequest request);

        Bill Checkout(Guid orderId);
    }

    public class OrderItemRequest
    {
        public Guid OrderId { get; set; }

        public int GuestNumber { get; set; }

        public DishEnum Dish { get; set; }

        public int Count { get; set; }
    }
}