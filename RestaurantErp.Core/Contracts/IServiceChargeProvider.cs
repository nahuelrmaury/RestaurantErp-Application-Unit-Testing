using RestaurantErp.Core.Models.Bill;

namespace RestaurantErp.Core.Providers
{
    public interface IServiceChargeProvider
    {
        void ApplyServiceCharge(Bill bill);
    }
}
