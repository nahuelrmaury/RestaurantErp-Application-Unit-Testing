using FluentAssertions;
using Moq;
using NUnit.Framework;
using RestaurantErp.Core.Contracts;
using RestaurantErp.Core.Helpers;
using RestaurantErp.Core.Models.Bill;
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
        public void TwoProductsInOrder_Ckeckout_AmountIsCorrect()
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

            IOrderProvider orderProvider = new OrderProvider(
                (IPriceStorage)productProvider,
                new[] { (IDiscountProvider)discountManager },
                calculator,
                new TimeHelper(),
                new BillHelper(productProvider));

            var requestProduct1 = new AddProductRequest
            {
                Name = "Starter",
                Price = 4
            };

            var requestProduct2 = new AddProductRequest
            {
                Name = "Main",
                Price = 7
            };

            var productId1 = productProvider.AddProduct(requestProduct1);
            var productId2 = productProvider.AddProduct(requestProduct2);

            // Action

            var orderId = orderProvider.CreateOrder();

            orderProvider.AddItem(new OrderItemRequest
            {
                OrderId = orderId,
                Count = 1,
                ProductId = productId1
            });

            orderProvider.AddItem(new OrderItemRequest
            {
                OrderId = orderId,
                Count = 1,
                ProductId = productId2
            });

            var actual = orderProvider.Checkout(orderId);

            // Assert

            var expected = new BillExternal
            {
                Amount = 11,
                AmountDiscounted = 11,
                Discount = 0,
                OrderId = orderId,
                Items = new []
                {
                    new BillItemExternal
                    {
                        Amount = 4,
                        Discount = 0,
                        AmountDiscounted = 4,
                        PersonId = 0,
                        ProductName = requestProduct1.Name
                    },
                    new BillItemExternal
                    {
                        Amount = 7,
                        Discount = 0,
                        AmountDiscounted = 7,
                        ProductName = requestProduct2.Name
                    }
                }
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [TestCase(2.5, "2023-1-20T18:59:59+00:00", 0.3, 0, 0, 19, 0, 2.5, 0.75, 1.75)]
        [TestCase(2.5, "2023-1-20T19:00:00+00:00", 0.3, 0, 0, 19, 0, 2.5, 0.75, 1.75)]
        [TestCase(2.5, "2023-1-20T19:00:01+00:00", 0.3, 0, 0, 19, 0, 2.5, 0, 2.5)]
        [TestCase(25, "2023-1-20T18:59:59+00:00", 0.1, 0, 0, 19, 0, 25, 2.5, 22.5)]
        [TestCase(25, "2023-1-20T19:00:00+00:00", 0.1, 0, 0, 19, 0, 25, 2.5, 22.5)]
        [TestCase(25, "2023-1-20T19:00:01+00:00", 0.1, 0, 0, 19, 0, 25, 0, 25)]
        public void CreateOneProductAndDiscountByTime_Ckeckout_AmountIsCorrect(decimal productPrice, 
            DateTime currentTime, 
            decimal discountValue, 
            int discountStartTimeHour, 
            int discountStartTimeMinute, 
            int discountEndTimeHour, 
            int discountEndTimeMinute, 
            decimal expectedAmount, 
            decimal expectedDiscount, 
            decimal expectedAmountDiscounted)
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

            var timeHelper = new Mock<ITimeHelper>();
            timeHelper.Setup(mk => mk.DateTime).Returns(currentTime);

            IOrderProvider orderProvider = new OrderProvider(
                (IPriceStorage)productProvider,
                new[] { (IDiscountProvider)discountManager },
                calculator,
                timeHelper.Object,
                new BillHelper(productProvider));

            var productId = productProvider.AddProduct(new AddProductRequest
            {
                Name = "Starter",
                Price = productPrice
            });

            discountManager.Add(new DiscountByTimeSettings
            {
                ProductId = productId,
                StartTime = new TimeOnly(discountStartTimeHour, discountStartTimeMinute),
                EndTime = new TimeOnly(discountEndTimeHour, discountEndTimeMinute),
                DiscountValue = discountValue
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

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedAmount, bill.Amount);
                Assert.AreEqual(expectedAmountDiscounted, bill.AmountDiscounted);
                Assert.AreEqual(expectedDiscount, bill.Discount);
            });
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
                timeHelper,
                new BillHelper(productProvider));

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