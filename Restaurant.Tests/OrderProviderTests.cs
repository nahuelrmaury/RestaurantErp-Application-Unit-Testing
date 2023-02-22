using NUnit.Framework;
using RestaurantErp.Core;
using System.Linq;

namespace Restaurant.Tests
{
    public class OrderProviderTests
    {
        [Test]
        public void Test1()
        {
            // Precondition

            var priceStorage = new PriceStorage();
            var discountManager = new DiscountByTimeManager();
            
            var provider = new OrderProvider(priceStorage, new[] { discountManager });

            //discountManager.Add((order) =>
            //{
            //    order.Items.Where(i => i.Dish == DishEnum.Drink).ToList().ForEach(i => i.);

            //});

            // Action

            // Assert
        }
    }
}