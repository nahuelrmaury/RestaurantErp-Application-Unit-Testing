namespace RestaurantErp.Core
{
    public interface IPriceStorage
    {
        decimal GetProductPrice(DishEnum productType);
    }
}