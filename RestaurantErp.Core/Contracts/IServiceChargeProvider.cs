using RestaurantErp.Core.Models.Bill;

namespace RestaurantErp.Core.Contracts
{
    public interface IServiceChargeProvider
    {
        void ApplyServiceCharge(Bill bill);
    }
}
