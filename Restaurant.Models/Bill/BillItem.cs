namespace RestaurantErp.Core.Models.Bill
{
    public class BillItem
    {
        public Guid OrderItemId { get; set; }

        public Guid ProductId { get; set; }

        public int PersonId { get; set; }

        public decimal Amount { get; set; }

        public decimal Discount { get; set; }

        public string ProductName { get; set; }

        public decimal AmountDiscounted { get; set; }
    }
}