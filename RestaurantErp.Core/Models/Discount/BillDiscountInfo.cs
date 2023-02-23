namespace RestaurantErp.Core
{
    public class BillDiscountInfo
    {
        //public decimal DiscountAmount { get; set; }

        public IEnumerable<BillDiscountItemInfo> Items { get; set; }
    }
}
