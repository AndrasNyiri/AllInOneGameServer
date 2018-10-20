using System.Collections.Generic;
using LightEngineSerializeable.SerializableClasses.DatabaseModel;
using LightGameServer.Database.Utils;
using LightGameServer.Game.Model;

namespace LightGameServer.Database
{
    class ModelCreator
    {
        private QueryRepository _repo;

        public ModelCreator(QueryRepository repo)
        {
            this._repo = repo;
        }

        public PlayerData CreatePlayerData(Dictionary<string, object> value)
        {
            if (value.ContainsKey("device_id"))
            {
                return new PlayerData
                {
                    DeviceId = Encryptor.EncryptDeviceId(value.Get<uint>("device_id")),
                    PlayerId = value.Get<uint>("player_id"),
                    Name = value.Get<string>("name"),
                    Coin = value.Get<int>("coin"),
                    Diamond = value.Get<int>("diamond"),
                    LadderScore = value.Get<int>("ladder_score")
                };
            }
            return new PlayerData
            {
                PlayerId = value.Get<uint>("player_id"),
                Name = value.Get<string>("name"),
                Coin = value.Get<int>("coin"),
                Diamond = value.Get<int>("diamond"),
                LadderScore = value.Get<int>("ladder_score")
            };
        }

        public UnitSettings CreateUnitSettings(Dictionary<string, object> value)
        {
            return new UnitSettings
            {
                Id = value.Get<short>("id"),
                Name = value.Get<string>("name"),
                Density = value.Get<float>("density"),
                Radius = value.Get<float>("radius"),
                PushForce = value.Get<short>("push_force"),
                Hp = value.Get<short>("hp"),
                Damage = value.Get<short>("damage"),
                ProjectileDamage = value.Get<short>("projectile_damage")
            };
        }

        public SkillSettings CreateSkillSettings(Dictionary<string, object> value)
        {
            return new SkillSettings
            {
                Id = value.Get<short>("id"),
                Name = value.Get<string>("name"),
                Radius = value.Get<float>("radius"),
                Density = value.Get<float>("density"),
                Value1 = value.Get<float>("value1"),
                Value2 = value.Get<float>("value2"),
                Value3 = value.Get<float>("value3")
            };
        }
    }
}
