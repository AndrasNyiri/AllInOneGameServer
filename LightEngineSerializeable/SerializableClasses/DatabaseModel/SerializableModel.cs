using System;
using System.Collections.Generic;
using LightEngineSerializeable.LiteNetLib.Utils;
using LightEngineSerializeable.SerializableClasses.Enums;
using LightEngineSerializeable.Utils.Serializers;

namespace LightEngineSerializeable.SerializableClasses.DatabaseModel
{
    [System.Serializable]
    public abstract class SerializableModel
    {
        public virtual List<object> GetPropertyValues()
        {
            List<object> parameters = new List<object>();
            foreach (var property in GetType().GetProperties())
            {
                var obj = property.GetValue(this, null);
                if (obj == null) obj = "";
                parameters.Add(obj);
            }

            return parameters;
        }

        public virtual NetDataWriter Serialize(NetworkCommand command)
        {
            List<object> parameters = new List<object> { (byte)command };
            foreach (var property in GetType().GetProperties())
            {
                var obj = property.GetValue(this, null);
                if (obj == null) obj = "";
                parameters.Add(obj);
            }
            return ObjectSerializationUtil.SerializeObjects(parameters.ToArray());
        }

        public static T DeSerialize<T>(NetDataReader reader, bool dropCommand = false) where T : SerializableModel, new()
        {
            if (dropCommand) reader.GetByte();
            T model = new T();
            foreach (var property in model.GetType().GetProperties())
            {
                property.SetValue(model, Convert.ChangeType(reader.Get(property.PropertyType), property.PropertyType), null);
            }
            return model;
        }
    }
}
