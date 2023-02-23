namespace RestaurantErp.Core
{
    public class DiscountCalculator
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
            foreach (var itemInfo in discountInfo.Items)
            {
                var targetItem = bill.Items.Single(i => i.ItemId == itemInfo.ItemId);

                var itemCalculatedPrice = targetItem.Amount - itemInfo.DiscountAmount;

                targetItem.AmountDiscounted = itemCalculatedPrice < _settings.MinimalProductPrice ? _settings.MinimalProductPrice : itemCalculatedPrice;

                targetItem.Discount =  targetItem.Amount - targetItem.AmountDiscounted;
            }

            bill.Discount = bill.Items.Sum(i => i.Discount);
            bill.AmountDiscounted = bill.Items.Sum(i => i.Amount);
        }
    }
}