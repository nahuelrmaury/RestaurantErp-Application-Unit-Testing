namespace RestaurantErp.Core.Contracts
{
    public class TimeHelper : ITimeHelper
    {
        public DateTime DateTime => DateTime.UtcNow;
    }
}
