namespace RestaurantErp.Core
{
    public interface IDiscountByTimeProvider: IDiscountProvider
    {
        Guid Add(DiscountByTimeSettings settings);

        bool Remove(Guid id);

        IDictionary<Guid, DiscountByTimeSettings> GetAll();
    }
}
