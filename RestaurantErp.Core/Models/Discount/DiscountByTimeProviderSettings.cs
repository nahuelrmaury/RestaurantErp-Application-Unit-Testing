namespace RestaurantErp.Core
{
    public class DiscountByTimeProviderSettings
    {
        // We use EndDiscountDelay to compensate possible delays (network etc.)
        // between ordering item and adding information about that to ERP system
        public TimeSpan EndDiscountDelay { get; set; }
    }
}
