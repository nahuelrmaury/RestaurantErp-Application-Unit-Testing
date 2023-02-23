using NUnit.Framework;
using RestaurantErp.Core;
using RestaurantErp.Core.Contracts;
using RestaurantErp.Core.Models.Discount;
using RestaurantErp.Core.Models.Order;
using RestaurantErp.Core.Models.Product;
using RestaurantErp.Core.Providers;
using System;
using System.Linq;

namespace Restaurant.Tests
{
    public class OrderProviderTests
    {
        [Test]
        public void Test1()
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
                EndTime = new TimeOnly(19, 0)
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
                ProductId = starterId
            });

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
                ProductId = starterId
            });

            // Assert
        }
    }
}