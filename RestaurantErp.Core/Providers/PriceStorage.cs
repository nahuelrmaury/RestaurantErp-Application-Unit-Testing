using RestaurantErp.Core.Contracts;
using RestaurantErp.Core.Enums;
using System.Collections.Concurrent;

namespace RestaurantErp.Core.Providers
{
    public class PriceStorage : IPriceStorageV1
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
    // Assumption:
    // creation of product and setting/updating of price should be splitted
    public class ProductProvider : IProductProvider, IPriceStorage
    {
        private ConcurrentDictionary<Guid, ProductInfo> _productInfoById = new ConcurrentDictionary<Guid, ProductInfo>();

        public Guid AddProduct(ProductInfo productInfo)
        {
            var id = Guid.NewGuid();

            var pi = new ProductInfo
            {
                Id = id,
                Name = productInfo.Name,
                Price = productInfo.Price
            };

            _productInfoById[id] = pi;

            return id;
        }

        public ProductInfo GetProductInfo(Guid id)
        {
            if (!_productInfoById.TryGetValue(id, out var value))
                throw new ArgumentOutOfRangeException($"Wrong productId: '{id}'");

            return value;
        }

        public decimal GetProductPrice(Guid id)
        {
            var product = GetProductInfo(id);

            return product.Price;
        }
    }

    public interface IProductProvider
    {
        public Guid AddProduct(ProductInfo productInfo);
    }

    public class ProductInfo
    {
        public Guid Id { get; set; }

        public string Name { get; set; }    

        public decimal Price { get; set; }    
    }

    public class AddProductRequest
    {
        public string Name { get; set; }

        public decimal Price { get; set; }
    }
}