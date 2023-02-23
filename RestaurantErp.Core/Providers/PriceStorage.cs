using RestaurantErp.Core.Contracts;
using RestaurantErp.Core.Enums;
using System.Collections.Concurrent;

namespace RestaurantErp.Core.Providers
{
    public class PriceStorage : IPriceStorage
    {
        private ConcurrentDictionary<DishEnum, decimal> _priceByProduct = new ConcurrentDictionary<DishEnum, decimal>()
        {
            [DishEnum.Starter] = 4,
            [DishEnum.Main] = 7,
            [DishEnum.Drink] = 2.5m
        };

        public decimal GetProductPrice(DishEnum productType)
        {
            var isProductExist = _priceByProduct[productType];

            if (!_priceByProduct.TryGetValue(productType, out var price))
                throw new ArgumentOutOfRangeException($"Price Storage does not contain information about dish '{productType}'");

            return price;
        }
    }
}