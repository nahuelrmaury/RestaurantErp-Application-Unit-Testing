using RestaurantErp.Core.Models.Bill;
using RestaurantErp.Core.Models.Discount;

namespace RestaurantErp.Core.Contracts
{
    public interface IDiscountCalculator
    {
        void ApplyDiscount(Bill bill, IEnumerable<BillDiscountInfo> discountInfo);

        void ApplyDiscount(Bill bill, BillDiscountInfo discountInfo);
    }
}
