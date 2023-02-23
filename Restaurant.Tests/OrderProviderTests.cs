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
                new BillHelper(productProvider),
                new ServiceChargeProvider(new ServiceChargeProviderSettings { ServiceRate = 0.1m}));

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
                Service = 1.1m,
                Total = 12.1m,
                Items = new[]
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
                new BillHelper(productProvider),
                new ServiceChargeProvider(new ServiceChargeProviderSettings { ServiceRate = 0.1m }));

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
        public void SeveralItemsWithDiscountSeveralTooLate_Checkout_BillIsCorrect()
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

            IOrderProvider orderProvider = new OrderProvider(
                (IPriceStorage)productProvider,
                new[] { (IDiscountProvider)discountManager },
                calculator,
                timeHelper.Object,
                new BillHelper(productProvider),
                new ServiceChargeProvider(new ServiceChargeProviderSettings { ServiceRate = 0.1m }));

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

            var requestProduct3 = new AddProductRequest
            {
                Name = "Drink",
                Price = 2.5m
            };


            var productId1 = productProvider.AddProduct(requestProduct1);
            var productId2 = productProvider.AddProduct(requestProduct2);
            var productId3 = productProvider.AddProduct(requestProduct3);

            discountManager.Add(new DiscountByTimeSettings
            {
                ProductId = productId3,
                StartTime = new TimeOnly(0, 0),
                EndTime = new TimeOnly(19, 0),
                DiscountValue = 0.3m
            });

            timeHelper.Setup(mk => mk.DateTime).Returns(DateTime.Parse("2023-1-20T18:59:59+00:00"));

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
                Count = 2,
                ProductId = productId2
            });

            orderProvider.AddItem(new OrderItemRequest
            {
                OrderId = orderId,
                Count = 2,
                ProductId = productId3
            });

            timeHelper.Setup(mk => mk.DateTime).Returns(DateTime.Parse("2023-1-20T20:00:00+00:00"));

            orderProvider.AddItem(new OrderItemRequest
            {
                OrderId = orderId,
                Count = 2,
                ProductId = productId2
            });

            orderProvider.AddItem(new OrderItemRequest
            {
                OrderId = orderId,
                Count = 2,
                ProductId = productId3
            });

            var actual = orderProvider.Checkout(orderId);

            //Assert

            var expected = new BillExternal
            {
                Amount = 42m,
                AmountDiscounted = 40.5m,
                Discount = 1.5m,
                OrderId = orderId,
                Service = 4.05m,
                Total = 44.55m,
                Items = new[]
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
                        PersonId = 0,
                        ProductName = requestProduct2.Name
                    },
                    new BillItemExternal
                    {
                        Amount = 7,
                        Discount = 0,
                        AmountDiscounted = 7,
                        ProductName = requestProduct2.Name
                    },
                    new BillItemExternal
                    {
                        Amount = 7,
                        Discount = 0,
                        AmountDiscounted = 7,
                        PersonId = 0,
                        ProductName = requestProduct2.Name
                    },
                    new BillItemExternal
                    {
                        Amount = 7,
                        Discount = 0,
                        AmountDiscounted = 7,
                        ProductName = requestProduct2.Name
                    },
                    new BillItemExternal
                    {
                        Amount = 2.5m,
                        Discount = 0,
                        AmountDiscounted = 2.5m,
                        PersonId = 0,
                        ProductName = requestProduct3.Name
                    },
                    new BillItemExternal
                    {
                        Amount = 2.5m,
                        Discount = 0,
                        AmountDiscounted = 2.5m,
                        ProductName = requestProduct3.Name
                    },
                    new BillItemExternal
                    {
                        Amount = 2.5m,
                        Discount = 0.75m,
                        AmountDiscounted = 1.75m,
                        PersonId = 0,
                        ProductName = requestProduct3.Name
                    },
                    new BillItemExternal
                    {
                        Amount = 2.5m,
                        Discount = 0.75m,
                        AmountDiscounted = 1.75m,
                        ProductName = requestProduct3.Name
                    }
                }
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void ThreeProductsBy4Items_Checkout_BillIsCorrect()
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
                new BillHelper(productProvider),
                new ServiceChargeProvider(new ServiceChargeProviderSettings { ServiceRate = 0.1m }));

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

            var requestProduct3 = new AddProductRequest
            {
                Name = "Drink",
                Price = 2.5m
            };

            var productId1 = productProvider.AddProduct(requestProduct1);
            var productId2 = productProvider.AddProduct(requestProduct2);
            var productId3 = productProvider.AddProduct(requestProduct3);

            // Action

            var orderId = orderProvider.CreateOrder();

            orderProvider.AddItem(new OrderItemRequest
            {
                OrderId = orderId,
                Count = 4,
                ProductId = productId1
            });

            orderProvider.AddItem(new OrderItemRequest
            {
                OrderId = orderId,
                Count = 4,
                ProductId = productId2
            });

            orderProvider.AddItem(new OrderItemRequest
            {
                OrderId = orderId,
                Count = 4,
                ProductId = productId3
            });

            var actual = orderProvider.Checkout(orderId);

            //Assert

            var expected = new BillExternal
            {
                Amount = 54,
                AmountDiscounted = 54,
                Discount = 0,
                OrderId = orderId,
                Service = 5.4m,
                Total = 59.4m,
                Items = new[]
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
                        Amount = 4,
                        Discount = 0,
                        AmountDiscounted = 4,
                        PersonId = 0,
                        ProductName = requestProduct1.Name
                    },
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
                        PersonId = 0,
                        ProductName = requestProduct2.Name
                    },
                    new BillItemExternal
                    {
                        Amount = 7,
                        Discount = 0,
                        AmountDiscounted = 7,
                        PersonId = 0,
                        ProductName = requestProduct2.Name
                    },
                    new BillItemExternal
                    {
                        Amount = 7,
                        Discount = 0,
                        AmountDiscounted = 7,
                        PersonId = 0,
                        ProductName = requestProduct2.Name
                    },
                    new BillItemExternal
                    {
                        Amount = 7,
                        Discount = 0,
                        AmountDiscounted = 7,
                        PersonId = 0,
                        ProductName = requestProduct2.Name
                    },
                    new BillItemExternal
                    {
                        Amount = 2.5m,
                        Discount = 0,
                        AmountDiscounted = 2.5m,
                        PersonId = 0,
                        ProductName = requestProduct3.Name
                    },
                    new BillItemExternal
                    {
                        Amount = 2.5m,
                        Discount = 0,
                        AmountDiscounted = 2.5m,
                        PersonId = 0,
                        ProductName = requestProduct3.Name
                    },
                    new BillItemExternal
                    {
                        Amount = 2.5m,
                        Discount = 0,
                        AmountDiscounted = 2.5m,
                        PersonId = 0,
                        ProductName = requestProduct3.Name
                    },
                    new BillItemExternal
                    {
                        Amount = 2.5m,
                        Discount = 0,
                        AmountDiscounted = 2.5m,
                        PersonId = 0,
                        ProductName = requestProduct3.Name
                    }
                }
            };

            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void ThreeProductsBy4Items_CancellAllProductsBy1QtyAndCheckout_BillIsCorrect()
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
                new BillHelper(productProvider),
                new ServiceChargeProvider(new ServiceChargeProviderSettings { ServiceRate = 0.1m }));

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

            var requestProduct3 = new AddProductRequest
            {
                Name = "Drink",
                Price = 2.5m
            };

            var productId1 = productProvider.AddProduct(requestProduct1);
            var productId2 = productProvider.AddProduct(requestProduct2);
            var productId3 = productProvider.AddProduct(requestProduct3);

            // Action

            var orderId = orderProvider.CreateOrder();

            orderProvider.AddItem(new OrderItemRequest
            {
                OrderId = orderId,
                Count = 4,
                ProductId = productId1
            });

            orderProvider.AddItem(new OrderItemRequest
            {
                OrderId = orderId,
                Count = 4,
                ProductId = productId2
            });

            orderProvider.AddItem(new OrderItemRequest
            {
                OrderId = orderId,
                Count = 4,
                ProductId = productId3
            });

            orderProvider.CancelItem(new OrderItemRequest
            {
                OrderId = orderId,
                Count = 1,
                ProductId = productId1
            });

            orderProvider.CancelItem(new OrderItemRequest
            {
                OrderId = orderId,
                Count = 1,
                ProductId = productId2
            });

            orderProvider.CancelItem(new OrderItemRequest
            {
                OrderId = orderId,
                Count = 1,
                ProductId = productId3
            });

            var actual = orderProvider.Checkout(orderId);

            //Assert

            var expected = new BillExternal
            {
                Amount = 40.5m,
                AmountDiscounted = 40.5m,
                Discount = 0,
                OrderId = orderId,
                Service = 4.05m,
                Total = 44.55m,
                Items = new[]
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
                        Amount = 4,
                        Discount = 0,
                        AmountDiscounted = 4,
                        PersonId = 0,
                        ProductName = requestProduct1.Name
                    },
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
                        PersonId = 0,
                        ProductName = requestProduct2.Name
                    },
                    new BillItemExternal
                    {
                        Amount = 7,
                        Discount = 0,
                        AmountDiscounted = 7,
                        PersonId = 0,
                        ProductName = requestProduct2.Name
                    },
                    new BillItemExternal
                    {
                        Amount = 7,
                        Discount = 0,
                        AmountDiscounted = 7,
                        PersonId = 0,
                        ProductName = requestProduct2.Name
                    },
                    new BillItemExternal
                    {
                        Amount = 2.5m,
                        Discount = 0,
                        AmountDiscounted = 2.5m,
                        PersonId = 0,
                        ProductName = requestProduct3.Name
                    },
                    new BillItemExternal
                    {
                        Amount = 2.5m,
                        Discount = 0,
                        AmountDiscounted = 2.5m,
                        PersonId = 0,
                        ProductName = requestProduct3.Name
                    },
                    new BillItemExternal
                    {
                        Amount = 2.5m,
                        Discount = 0,
                        AmountDiscounted = 2.5m,
                        PersonId = 0,
                        ProductName = requestProduct3.Name
                    }
                }
            };

            actual.Should().BeEquivalentTo(expected);
        }
    }
}