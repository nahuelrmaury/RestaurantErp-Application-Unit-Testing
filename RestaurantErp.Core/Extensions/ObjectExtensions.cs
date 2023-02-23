using Newtonsoft.Json;

namespace RestaurantErp.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static T DeepCopy<T>(this T instance)
        {
            string serialized = JsonConvert.SerializeObject(instance);
            T deserialized = JsonConvert.DeserializeObject<T>(serialized);

            return deserialized;
        }
    }
}
