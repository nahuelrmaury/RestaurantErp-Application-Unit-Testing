namespace RestaurantErp.Core.Models.Bill
{
    public class Bill
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public IEnumerable<BillItem> Items { get; set; }

        public decimal Amount { get; set; }

        public decimal Discount { get; set; }

        public decimal AmountDiscounted { get; set; }

        public decimal Service { get; set; }

        public decimal Total { get; set; }
    }
}