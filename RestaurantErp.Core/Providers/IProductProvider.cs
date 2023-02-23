using RestaurantErp.Core.Models.Product;

namespace RestaurantErp.Core.Providers
{
    public interface IProductProvider
    {
        public Guid AddProduct(AddProductRequest productInfo);
    }
}