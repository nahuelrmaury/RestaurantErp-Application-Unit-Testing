using RestaurantErp.Core.Enums;

namespace RestaurantErp.Core.Models.Discount
{
    public class DiscountByTimeSettings
    {
        public DishEnum Dish { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public decimal DiscountValue { get; set; }
    }
}
