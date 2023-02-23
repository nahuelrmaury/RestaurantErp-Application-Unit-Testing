using RestaurantErp.Core.Contracts;
using RestaurantErp.Core.Models.Product;
using System.Collections.Concurrent;

namespace RestaurantErp.Core.Providers
{
    // Assumption:
    // creation of product and setting/updating of price should be splitted
    public class ProductProvider : IProductProvider, IPriceStorage
    {
        private ConcurrentDictionary<Guid, ProductInfo> _productInfoById = new ConcurrentDictionary<Guid, ProductInfo>();

        public Guid AddProduct(AddProductRequest request)
        {
            var id = Guid.NewGuid();

            var pi = new ProductInfo
            {
                Id = id,
                Name = request.Name,
                Price = request.Price
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
}