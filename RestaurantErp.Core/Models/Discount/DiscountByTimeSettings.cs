namespace RestaurantErp.Core
{
    public class DiscountByTimeSettings
    {
        public DishEnum Dish { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public decimal DiscountValue { get; set; }
    }
}
