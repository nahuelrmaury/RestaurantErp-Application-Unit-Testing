using FluentAssertions;
using NUnit.Framework;
using RestaurantErp.Core.Contracts;
using RestaurantErp.Core.Helpers;
using RestaurantErp.Core.Models;
using RestaurantErp.Core.Models.Bill;
using RestaurantErp.Core.Models.Discount;
using RestaurantErp.Core.Models.Order;
using RestaurantErp.Core.Models.Product;
using RestaurantErp.Core.Providers;
using RestaurantErp.Core.Providers.Discount;
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