using RestaurantErp.Core.Contracts;
using RestaurantErp.Core.Models;
using RestaurantErp.Core.Models.Bill;

namespace RestaurantErp.Core.Providers
{
    public class ServiceChargeProvider: IServiceChargeProvider
    {
        private readonly ServiceChargeProviderSettings _settings;

        public ServiceChargeProvider(ServiceChargeProviderSettings settings)
        {
            _settings = settings;
        }

        public void ApplyServiceCharge(Bill bill)
        {
            bill.Service = bill.AmountDiscounted * _settings.ServiceRate;

            bill.Total = bill.AmountDiscounted + bill.Service;
        }
    }
}
