using RestaurantErp.Core.Models.Discount;

namespace RestaurantErp.Core.Contracts
{
    public interface IDiscountByTimeProvider : IDiscountProvider
    {
        Guid Add(DiscountByTimeSettings settings);

        bool Remove(Guid id);

        IDictionary<Guid, DiscountByTimeSettings> GetAll();
    }
}
