using System;
using System.Globalization;
using LightEngineSerializeable.SerializableClasses;
using LightEngineSerializeable.SerializableClasses.DatabaseModel;

namespace LightGameServer.Database
{
    class DataStore
    {
        public StaticData Data { get; }

        public DataStore(QueryRepository queryRepository)
        {
            Data = new StaticData
            {
                TimeStamp = DateTime.UtcNow.ToString(CultureInfo.CurrentCulture),
                UnitSettings = queryRepository.GetAllUnitSettings(),
                SkillSettings = queryRepository.GetAllSkillSettings()
            };
        }

        public UnitSettings GetUnitSettings(string unitName)
        {
            foreach (var unitSetting in Data.UnitSettings)
            {
                if (unitSetting.Name == unitName) return unitSetting;
            }
            return null;
        }

        public SkillSettings GetSkillSettings(string skillName)
        {
            foreach (var skillSetting in Data.SkillSettings)
            {
                if (skillSetting.Name == skillName) return skillSetting;
            }
            return null;
        }
    }
}
