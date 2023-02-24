namespace RestaurantErp.Core.Models.Discount
{
    public class DiscountByTimeSettings
    {
        public Guid ProductId { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public decimal DiscountValue { get; set; }
    }
}
