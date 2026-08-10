using System.Text.Json;

namespace GlassesShop.Helpers
{
    public static class SessionHelper
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T? GetObject<T>(this ISession session, string key)
        {
            var data = session.GetString(key);
            return string.IsNullOrEmpty(data) ? default : JsonSerializer.Deserialize<T>(data);
        }
    }
}