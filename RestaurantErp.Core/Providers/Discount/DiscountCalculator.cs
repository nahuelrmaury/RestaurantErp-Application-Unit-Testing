using RestaurantErp.Core.Contracts;
using RestaurantErp.Core.Models.Bill;
using RestaurantErp.Core.Models.Discount;

namespace RestaurantErp.Core.Providers
{
    public class DiscountCalculator: IDiscountCalculator
    {
        private DiscountCalculatorSettings _settings;

        public DiscountCalculator(DiscountCalculatorSettings settings)
        {
            _settings = settings;
        }

        public void ApplyDiscount(Bill bill, IEnumerable<BillDiscountInfo> discountInfo)
        {
            var accumulated = AccumulateDiscounts(discountInfo);

            ApplyDiscount(bill, accumulated);
        }

        private BillDiscountInfo AccumulateDiscounts(IEnumerable<BillDiscountInfo> discountCollection)
        {
            var items = discountCollection
                .Select(i => i.Items)
                .SelectMany(i => i)
                .GroupBy(i => i.ItemId)
                .Select(i => new BillDiscountItemInfo
                {
                    ItemId = i.Key,
                    DiscountAmount = i.Sum(i => i.DiscountAmount)
                });

            return new BillDiscountInfo
            {
                Items = items
            };
        }

        public void ApplyDiscount(Bill bill, BillDiscountInfo discountInfo)
        {
            foreach (var discountItemInfo in discountInfo.Items)
            {
                var billItem = bill.Items.Single(i => i.OrderItemId == discountItemInfo.ItemId);

                var itemCalculatedPrice = billItem.Amount - discountItemInfo.DiscountAmount;

                billItem.AmountDiscounted = itemCalculatedPrice < _settings.MinimalProductPrice ? _settings.MinimalProductPrice : itemCalculatedPrice;

                billItem.Discount = billItem.Amount - billItem.AmountDiscounted;
            }

            bill.Discount = bill.Items.Sum(i => i.Discount);
            bill.AmountDiscounted = bill.Items.Sum(i => i.Amount);
        }
    }
}