namespace RestaurantErp.Core.Contracts
{
    public interface IPriceStorage
    {
        decimal GetProductPrice(Guid id);
    }
}