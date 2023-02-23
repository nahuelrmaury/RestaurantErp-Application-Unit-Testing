using RestaurantErp.Core.Contracts;
using RestaurantErp.Core.Models.Bill;
using RestaurantErp.Core.Models.Order;
using System.Collections.Concurrent;

namespace RestaurantErp.Core.Providers
{
    public class OrderProvider : IOrderProvider
    {
        private readonly ConcurrentBag<Order> _orderStorage = new ConcurrentBag<Order>();

        private readonly IPriceStorageV1 _priceStorage;
        private readonly IEnumerable<IDiscountProvider> _discountProviders;
        private readonly DiscountCalculator _discountCalculator;

        public OrderProvider(IPriceStorageV1 priceStorage,
            IEnumerable<IDiscountProvider> discountProviders,
            DiscountCalculator discountCalculator)
        {
            _priceStorage = priceStorage;
            _discountProviders = discountProviders;
            _discountCalculator = discountCalculator;
        }

        public Guid CreateOrder()
        {
            var order = new Order
            {
                Id = Guid.NewGuid()
            };

            _orderStorage.Add(order);

            return order.Id;
        }

        // Assumption:
        // order item should be added imediatly after client order, so we can calculate DateTime on server side
        public void AddItem(OrderItemRequest request)
        {
            var targetOrder = _orderStorage.Single(i => i.Id == request.OrderId);

            var newItem = new OrderItem
            {
                ItemId = request.OrderId,
                Price = _priceStorage.GetProductPrice(request.Dish),
                PersonId = request.GuestNumber,
                Dish = request.Dish,
                OrderingTime = DateTime.UtcNow,
            };

            for (var _ = 0; _ < request.Count; _++)
                targetOrder.Items.Add(newItem);
        }

        public void CancelItem(OrderItemRequest request)
        {
            var targetOrder = _orderStorage.Single(i => i.Id == request.OrderId);

            // Assumption:
            // we should cancell all items in order of their ordering time
            var targetItems = targetOrder.Items
                .Where(i => i.Dish == request.Dish
                    && request.GuestNumber == request.GuestNumber
                    && !i.IsCancelled)
                .OrderBy(i => i.OrderingTime)
                .Take(request.Count);

            if (targetItems.Count() < request.Count)
                throw new ArgumentOutOfRangeException($"Gurst '{request.GuestNumber}' try to remove '{request.Count}' dishes '{request.Dish}', but order contains only '{targetItems.Count()}' of them");

            foreach (var targetitem in targetItems)
            {
                targetitem.IsCancelled = true;
            }
        }


        // TODO:
        // add calback logic.
        // after changing order (AddItem / CancelItem methods) - previous bill must be compromised
        public Bill Checkout(Guid orderId)
        {
            var targetOrder = _orderStorage.Single(i => i.Id == orderId);

            var billItems = targetOrder.Items
                .Where(i => !i.IsCancelled)
                .Select(i => new BillItem
                {
                    Dish = i.Dish,
                    PersonId = i.PersonId,
                    Amount = i.Price,
                    AmountDiscounted = i.Price
                });

            var bill = new Bill
            {
                OrderId = orderId,
                Items = billItems,
                Amount = billItems.Sum(i => i.Amount),
                AmountDiscounted = billItems.Sum(i => i.AmountDiscounted)
            };

            var discounts = _discountProviders.Select(i => i.Calculate(targetOrder));

            _discountCalculator.ApplyDiscount(bill, discounts);

            return bill;
        }
    }
}