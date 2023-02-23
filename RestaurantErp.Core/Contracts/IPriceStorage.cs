using RestaurantErp.Core.Enums;

namespace RestaurantErp.Core.Contracts
{
    public interface IPriceStorage
    {
        decimal GetProductPrice(DishEnum productType);
    }
}