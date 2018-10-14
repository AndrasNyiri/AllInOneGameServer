using System;
using System.Collections.Generic;
using System.Globalization;
using LightEngineSerializeable.LiteNetLib.Utils;
using LightEngineSerializeable.SerializableClasses.DatabaseModel;
using LightEngineSerializeable.SerializableClasses.Enums;
using LightEngineSerializeable.Utils.Serializers;

namespace LightEngineSerializeable.SerializableClasses
{
    [System.Serializable]
    public class StaticData : SerializableModel
    {
        public string TimeStamp { get; set; }

        public UnitSettings[] UnitSettings { get; set; }
        public SkillSettings[] SkillSettings { get; set; }

        public override NetDataWriter Serialize(NetworkCommand command, params object[] extra)
        {
            List<object> parameters = new List<object> { (byte)command };
            parameters.AddRange(extra);
            parameters.Add(TimeStamp);
            parameters.Add((byte)UnitSettings.Length);
            foreach (var unit in UnitSettings)
            {
                parameters.AddRange(unit.GetPropertyValues());
            }
            parameters.Add((byte)SkillSettings.Length);
            foreach (var skill in SkillSettings)
            {
                parameters.AddRange(skill.GetPropertyValues());
            }
            return ObjectSerializationUtil.SerializeObjects(parameters.ToArray());
        }

        public static StaticData DeSerialize(NetDataReader reader, bool dropCommand = false)
        {
            if (dropCommand) reader.GetByte();
            string timeStamp = reader.GetString();
            List<UnitSettings> unitSettings = new List<UnitSettings>();
            List<SkillSettings> skillSettings = new List<SkillSettings>();

            byte length = reader.GetByte();
            for (int i = 0; i < length; i++)
            {
                unitSettings.Add(DeSerialize<UnitSettings>(reader));
            }
            length = reader.GetByte();
            for (int i = 0; i < length; i++)
            {
                skillSettings.Add(DeSerialize<SkillSettings>(reader));
            }
            return new StaticData
            {
                TimeStamp = timeStamp,
                UnitSettings = unitSettings.ToArray(),
                SkillSettings = skillSettings.ToArray()
            };
        }
    }
}
