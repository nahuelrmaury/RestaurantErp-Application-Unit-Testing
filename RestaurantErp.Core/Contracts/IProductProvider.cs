using RestaurantErp.Core.Models.Product;

namespace RestaurantErp.Core.Contracts
{
    public interface IProductProvider
    {
        Guid AddProduct(AddProductRequest productInfo);

        ProductInfo GetProductInfo(Guid id);
    }
}