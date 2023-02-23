using RestaurantErp.Core.Models.Discount;
using RestaurantErp.Core.Models.Order;

namespace RestaurantErp.Core.Contracts
{
    public interface IDiscountProvider
    {
        BillDiscountInfo Calculate(Order order);
    }
}
