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
            uint playerId = value.Get<uint>("player_id");
            if (value.ContainsKey("device_id"))
            {
                return new PlayerData
                {
                    DeviceId = Encryptor.EncryptDeviceId(value.Get<uint>("device_id")),
                    PlayerId = playerId,
                    Name = value.Get<string>("name"),
                    Coin = value.Get<int>("coin"),
                    Diamond = value.Get<int>("diamond"),
                    LadderScore = value.Get<int>("ladder_score"),
                    OwnedUnits = _repo.GetPlayerUnits(playerId),
                    Deck = _repo.GetPlayerDeck(playerId)
                };
            }
            return new PlayerData
            {
                PlayerId = playerId,
                Name = value.Get<string>("name"),
                Coin = value.Get<int>("coin"),
                Diamond = value.Get<int>("diamond"),
                LadderScore = value.Get<int>("ladder_score"),
                OwnedUnits = _repo.GetPlayerUnits(playerId),
                Deck = _repo.GetPlayerDeck(playerId)
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

        public PlayerUnit CreatePlayerUnit(Dictionary<string, object> value)
        {
            return new PlayerUnit
            {
                UnitId = value.Get<short>("unit_id"),
                Amount = value.Get<int>("amount")
            };
        }

        public PlayerDeck CreatePlayerDeck(Dictionary<string, object> value)
        {
            return new PlayerDeck
            {
                UnitOne = value.Get<short>("unit_one_id"),
                UnitTwo = value.Get<short>("unit_two_id"),
                UnitThree = value.Get<short>("unit_three_id"),
                UnitFour = value.Get<short>("unit_four_id"),
            };
        }
    }
}
