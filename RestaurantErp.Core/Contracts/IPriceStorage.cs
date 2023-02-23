using RestaurantErp.Core.Enums;

namespace RestaurantErp.Core.Contracts
{
    public interface IPriceStorageV1
    {
        decimal GetProductPrice(DishEnum productType);
    }

    public interface IPriceStorage
    {
        decimal GetProductPrice(Guid id);
    }
}