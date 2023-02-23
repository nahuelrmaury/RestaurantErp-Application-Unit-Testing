using RestaurantErp.Core.Contracts;
using RestaurantErp.Core.Models.Bill;
using RestaurantErp.Core.Models.Order;
using System.Collections.Concurrent;

namespace RestaurantErp.Core.Providers
{
    public class OrderProvider : IOrderProvider
    {
        private readonly ConcurrentBag<Order> _orderStorage = new ConcurrentBag<Order>();

        private readonly IPriceStorage _priceStorage;
        private readonly IEnumerable<IDiscountProvider> _discountProviders;
        private readonly IDiscountCalculator _discountCalculator;
        private readonly ITimeHelper _timeHelper;

        public OrderProvider(IPriceStorage priceStorage,
            IEnumerable<IDiscountProvider> discountProviders,
            IDiscountCalculator discountCalculator,
            ITimeHelper timeHelper)
        {
            _priceStorage = priceStorage;
            _discountProviders = discountProviders;
            _discountCalculator = discountCalculator;
            _timeHelper = timeHelper;
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

            for (var _ = 0; _ < request.Count; _++)
            {
                var newItem = new OrderItem
                {
                    ItemId = Guid.NewGuid(),
                    Price = _priceStorage.GetProductPrice(request.ProductId),
                    PersonId = request.GuestNumber,
                    ProductId = request.ProductId,
                    OrderingTime = _timeHelper.DateTime
                };

                targetOrder.Items.Add(newItem);
            }
        }

        public void CancelItem(OrderItemRequest request)
        {
            var targetOrder = _orderStorage.Single(i => i.Id == request.OrderId);

            // Assumption:
            // we should cancell all items in order of their ordering time
            var targetItems = targetOrder.Items
                .Where(i => i.ProductId == request.ProductId
                    && request.GuestNumber == request.GuestNumber
                    && !i.IsCancelled)
                .OrderBy(i => i.OrderingTime)
                .Take(request.Count);

            if (targetItems.Count() < request.Count)
                throw new ArgumentOutOfRangeException($"Gurst '{request.GuestNumber}' try to remove '{request.Count}' dishes '{request.ProductId}', but order contains only '{targetItems.Count()}' of them");

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
                    ProductId = i.ProductId,
                    OrderItemId = i.ItemId,
                    PersonId = i.PersonId,
                    Amount = i.Price,
                    AmountDiscounted = i.Price
                })
                .ToArray();

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