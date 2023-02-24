namespace RestaurantErp.Core.Models.Bill
{
    public class BillItemExternal
    {
        public string ProductName { get; set; }

        public int PersonId { get; set; }

        public decimal Amount { get; set; }

        public decimal Discount { get; set; }

        public decimal AmountDiscounted { get; set; }
    }
}