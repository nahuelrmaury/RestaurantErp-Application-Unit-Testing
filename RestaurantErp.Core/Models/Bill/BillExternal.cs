namespace RestaurantErp.Core.Models.Bill
{
    public class BillExternal
    {
        public Guid OrderId { get; set; }

        public IEnumerable<BillItemExternal> Items { get; set; }

        public decimal Amount { get; set; }

        public decimal Discount { get; set; }

        public decimal AmountDiscounted { get; set; }

        public decimal Service { get; set; }

        public decimal Total { get; set; }
    }
}