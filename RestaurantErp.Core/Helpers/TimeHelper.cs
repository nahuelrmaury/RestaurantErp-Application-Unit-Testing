using RestaurantErp.Core.Contracts;

namespace RestaurantErp.Core.Helpers
{
    public class TimeHelper : ITimeHelper
    {
        public DateTime DateTime => DateTime.UtcNow;
    }
}
