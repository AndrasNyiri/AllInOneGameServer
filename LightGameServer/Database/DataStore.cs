using System;
using System.Globalization;
using LightEngineSerializeable.SerializableClasses;
using LightEngineSerializeable.SerializableClasses.DatabaseModel;

namespace LightGameServer.Database
{
    class DataStore
    {
        public StaticData Data { get; set; }

        private readonly QueryRepository _queryRepository;

        public DataStore(QueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
            Refresh();
        }

        public void Refresh()
        {
            Data = new StaticData
            {
                TimeStamp = DateTime.UtcNow.ToString(CultureInfo.CurrentCulture),
                UnitSettings = _queryRepository.GetAllUnitSettings(),
                SkillSettings = _queryRepository.GetAllSkillSettings()
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
