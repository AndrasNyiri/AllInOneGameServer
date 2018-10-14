using System;
using System.Collections.Generic;

namespace LightEngineSerializeable.Utils
{
    public class TypeSwitch
    {
        private readonly Dictionary<Type, Action<object>> _matches = new Dictionary<Type, Action<object>>();
        public TypeSwitch Case<T>(Action<T> action) { _matches.Add(typeof(T), x => action((T)x)); return this; }

        public void Switch(object x)
        {
            if (x != null && _matches.ContainsKey(x.GetType())) _matches[x.GetType()](x);
        }
    }
}
