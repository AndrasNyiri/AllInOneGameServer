using System;
using System.Collections.Generic;

namespace LightGameServer.Database.Utils
{
    public static class DictExtension
    {
        public static T Get<T>(this Dictionary<string, object> dict, string key)
        {
            object value = dict[key];
            if (value is T)
            {
                return (T)value;
            }
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (InvalidCastException)
            {
                return default(T);
            }
        }
    }
}
