using System.Collections.Generic;
using System.Linq;
using LightEngineSerializeable.SerializableClasses;
using LightEngineSerializeable.SerializableClasses.DatabaseModel;
using LightGameServer.Game.Model;
using LightGameServer.NetworkHandling;

namespace LightGameServer.Database
{
    class DataStore
    {
        public StaticData Data { get; }

        public DataStore(QueryRepository queryRepository)
        {
            Data = new StaticData
            {
                UnitSettings = queryRepository.GetAllUnitSettings()
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
    }
}
