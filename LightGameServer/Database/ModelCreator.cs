using System.Collections.Generic;
using LightGameServer.Database.Utils;

namespace LightGameServer.Database
{
    class ModelCreator
    {
        private QueryRepository _repo;

        public ModelCreator(QueryRepository repo)
        {
            this._repo = repo;
        }

        public PlayerData CreatePlayerData(Dictionary<string, object> values)
        {
            if (values.ContainsKey("device_id"))
            {
                return new PlayerData
                {
                    DeviceId = Encryptor.EncryptDeviceId(values.Get<ulong>("device_id")),
                    PlayerId = values.Get<ulong>("player_id"),
                    Name = values.Get<string>("name"),
                    Coin = values.Get<int>("coin"),
                    Diamond = values.Get<int>("diamond"),
                    LadderScore = values.Get<int>("ladder_score")
                };
            }
            return new PlayerData
            {
                PlayerId = values.Get<ulong>("player_id"),
                Name = values.Get<string>("name"),
                Coin = values.Get<int>("coin"),
                Diamond = values.Get<int>("diamond"),
                LadderScore = values.Get<int>("ladder_score")
            };
        }

    }
}
