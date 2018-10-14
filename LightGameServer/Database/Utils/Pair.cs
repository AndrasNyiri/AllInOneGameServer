using System.Collections.Generic;

namespace LightGameServer.Database.Utils
{
    public static class Pair
    {
        public static KeyValuePair<string, object> Of(string key, object value)
        {
            return new KeyValuePair<string, object>(key, value);
        }
    }
}
