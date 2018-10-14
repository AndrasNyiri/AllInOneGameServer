using System;
using System.Collections.Generic;
using LightEngineSerializeable.LiteNetLib.Utils;
using LightEngineSerializeable.SerializableClasses.DatabaseModel;
using LightEngineSerializeable.SerializableClasses.Enums;
using LightEngineSerializeable.Utils.Serializers;

namespace LightEngineSerializeable.SerializableClasses
{
    [System.Serializable]
    public class StaticData : SerializableModel
    {
        public UnitSettings[] UnitSettings { get; set; }

        public override NetDataWriter Serialize(NetworkCommand command)
        {
            List<object> parameters = new List<object> { (byte)command, (byte)UnitSettings.Length };
            foreach (var unit in UnitSettings)
            {
                parameters.AddRange(unit.GetPropertyValues());
            }
            return ObjectSerializationUtil.SerializeObjects(parameters.ToArray());
        }

        public static StaticData DeSerialize(NetDataReader reader, bool dropCommand = false)
        {
            if (dropCommand) reader.GetByte();
            List<UnitSettings> unitSettings = new List<UnitSettings>();
            byte length = reader.GetByte();
            for (int i = 0; i < length; i++)
            {
                unitSettings.Add(DeSerialize<UnitSettings>(reader));
            }

            return new StaticData { UnitSettings = unitSettings.ToArray() };
        }
    }
}
