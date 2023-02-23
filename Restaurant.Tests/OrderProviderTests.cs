using NUnit.Framework;
using RestaurantErp.Core.Contracts;
using RestaurantErp.Core.Models.Discount;
using RestaurantErp.Core.Models.Order;
using RestaurantErp.Core.Models.Product;
using RestaurantErp.Core.Providers;
using System;

namespace Restaurant.Tests
{
    public class OrderProviderTests
    {
        [Test]
        public void SeveralItems()
        {
            // Precondition

            var productProvider = new ProductProvider();

            IDiscountByTimeProvider discountManager = new DiscountByTimeProvider(new DiscountByTimeProviderSettings
            {
                EndDiscountDelay = TimeSpan.Zero
            });

            IDiscountCalculator calculator = new DiscountCalculator(new DiscountCalculatorSettings
            {
                MinimalProductPrice = 0
            });

            ITimeHelper timeHelper = new TimeHelper();

            IOrderProvider orderProvider = new OrderProvider(
                (IPriceStorage)productProvider,
                new[] { (IDiscountProvider)discountManager },
                calculator,
                timeHelper);

            var starterId = productProvider.AddProduct(new AddProductRequest
            {
                Name = "Starter",
                Price = 4
            });

            var mainId = productProvider.AddProduct(new AddProductRequest
            {
                Name = "Main",
                Price = 7
            });

            // Action

            var orderId = orderProvider.CreateOrder();

            orderProvider.AddItem(new OrderItemRequest
            {
                OrderId = orderId,
                Count = 1,
                ProductId = starterId
            });

            orderProvider.AddItem(new OrderItemRequest
            {
                OrderId = orderId,
                Count = 1,
                ProductId = mainId
            });

            var bill = orderProvider.Checkout(orderId);

            // Assert

            Assert.AreEqual(11, bill.Amount);
        }

        [Test]
        public void OneItemWithDiscountByTime()
        {
            // Precondition

            var productProvider = new ProductProvider();

            IDiscountByTimeProvider discountManager = new DiscountByTimeProvider(new DiscountByTimeProviderSettings
            {
                EndDiscountDelay = TimeSpan.Zero
            });

            IDiscountCalculator calculator = new DiscountCalculator(new DiscountCalculatorSettings
            {
                MinimalProductPrice = 0
            });

            ITimeHelper timeHelper = new TimeHelper();

            IOrderProvider orderProvider = new OrderProvider(
                (IPriceStorage)productProvider,
                new[] { (IDiscountProvider)discountManager },
                calculator,
                timeHelper);

            var productId = productProvider.AddProduct(new AddProductRequest
            {
                Name = "Starter",
                Price = 2.5m
            });

            discountManager.Add(new DiscountByTimeSettings
            {
                ProductId = productId,
                StartTime = new TimeOnly(0, 0),
                EndTime = new TimeOnly(19, 0),
                DiscountValue = 0.3m
            });

            // Action

            var orderId = orderProvider.CreateOrder();

            orderProvider.AddItem(new OrderItemRequest
            {
                OrderId = orderId,
                Count = 1,
                ProductId = productId
            });

            var bill = orderProvider.Checkout(orderId);

            // Assert

            Assert.AreEqual(2.5, bill.Amount);
            Assert.AreEqual(1.75, bill.AmountDiscounted);
            Assert.AreEqual(0.75, bill.Discount);
        }

        [Test]
        public void Test2()
        {
            // Precondition

            var productProvider = new ProductProvider();

            IDiscountByTimeProvider discountManager = new DiscountByTimeProvider(new DiscountByTimeProviderSettings
            {
                EndDiscountDelay = TimeSpan.Zero
            });

            IDiscountCalculator calculator = new DiscountCalculator(new DiscountCalculatorSettings
            {
                MinimalProductPrice = 0
            });

            ITimeHelper timeHelper = new TimeHelper();
            
            IOrderProvider orderProvider = new OrderProvider(
                (IPriceStorage)productProvider, 
                new[] {(IDiscountProvider)discountManager }, 
                calculator,
                timeHelper);

            var starterId = productProvider.AddProduct(new AddProductRequest
            {
                Name = "Starter",
                Price = 4
            });

            var mainId =  productProvider.AddProduct(new AddProductRequest
            {
                Name = "Main",
                Price = 7
            });

            var drinkId = productProvider.AddProduct(new AddProductRequest
            {
                Name = "Drink",
                Price = 2.5m
            });

            discountManager.Add(new DiscountByTimeSettings
            {
                ProductId = drinkId,
                StartTime = new TimeOnly(0, 0),
                EndTime = new TimeOnly(19, 0),
                DiscountValue = 0.3m
            });

            // Action

            var orderId = orderProvider.CreateOrder();

            orderProvider.AddItem(new OrderItemRequest
            {
                OrderId = orderId,
                Count = 4,
                ProductId = starterId
            });

            orderProvider.AddItem(new OrderItemRequest
            {
                OrderId = orderId,
                Count = 4,
                ProductId = mainId
            });

            orderProvider.AddItem(new OrderItemRequest
            {
                OrderId = orderId,
                Count = 4,
                ProductId = drinkId
            });

            var bill = orderProvider.Checkout(orderId);

            // Assert

            Assert.AreEqual(54, bill.Amount);
            Assert.AreEqual(51, bill.AmountDiscounted);
            Assert.AreEqual(3, bill.Discount);
        }
    }
}