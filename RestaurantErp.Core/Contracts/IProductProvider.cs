using RestaurantErp.Core.Models.Product;

namespace RestaurantErp.Core.Providers
{
    public interface IProductProvider
    {
        Guid AddProduct(AddProductRequest productInfo);

        ProductInfo GetProductInfo(Guid id);
    }
}