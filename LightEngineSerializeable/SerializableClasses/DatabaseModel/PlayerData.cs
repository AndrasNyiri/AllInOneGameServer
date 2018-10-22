using System.Collections.Generic;
using LightEngineSerializeable.LiteNetLib.Utils;
using LightEngineSerializeable.SerializableClasses.Enums;
using LightEngineSerializeable.Utils.Serializers;

namespace LightEngineSerializeable.SerializableClasses.DatabaseModel
{
    [System.Serializable]
    public class PlayerData : SerializableModel
    {
        public uint PlayerId { get; set; }
        public string DeviceId { get; set; }
        public string Name { get; set; }
        public int Coin { get; set; }
        public int Diamond { get; set; }
        public int LadderScore { get; set; }
        public PlayerUnit[] OwnedUnits { get; set; }
        public PlayerDeck Deck { get; set; }

        public override List<object> GetPropertyValues()
        {
            List<object> parameters = new List<object>();
            parameters.Add((byte)OwnedUnits.Length);
            foreach (var ownedUnit in OwnedUnits)
            {
                parameters.AddRange(ownedUnit.GetPropertyValues());
            }
            parameters.AddRange(Deck.GetPropertyValues());
            parameters.Add(PlayerId);
            parameters.Add(DeviceId);
            parameters.Add(Name);
            parameters.Add(Coin);
            parameters.Add(Diamond);
            parameters.Add(LadderScore);
            return parameters;
        }

        public override NetDataWriter Serialize(NetworkCommand command, params object[] extra)
        {
            List<object> parameters = new List<object> { (byte)command };
            parameters.AddRange(extra);
            parameters.Add((byte)OwnedUnits.Length);
            foreach (var ownedUnit in OwnedUnits)
            {
                parameters.AddRange(ownedUnit.GetPropertyValues());
            }
            parameters.AddRange(Deck.GetPropertyValues());
            parameters.Add(PlayerId);
            parameters.Add(DeviceId);
            parameters.Add(Name);
            parameters.Add(Coin);
            parameters.Add(Diamond);
            parameters.Add(LadderScore);
            return ObjectSerializationUtil.SerializeObjects(parameters.ToArray());
        }

        public static PlayerData DeSerialize(NetDataReader reader, bool dropCommand = false)
        {
            if (dropCommand) reader.GetByte();
            List<PlayerUnit> ownedUnits = new List<PlayerUnit>();
            var ownedUnitCount = reader.GetByte();
            for (int i = 0; i < ownedUnitCount; i++)
            {
                ownedUnits.Add(DeSerialize<PlayerUnit>(reader));
            }
            var deck = DeSerialize<PlayerDeck>(reader);
            return new PlayerData
            {
                PlayerId = reader.GetUInt(),
                DeviceId = reader.GetString(),
                Name = reader.GetString(),
                Coin = reader.GetInt(),
                Diamond = reader.GetInt(),
                LadderScore = reader.GetInt(),
                OwnedUnits = ownedUnits.ToArray(),
                Deck = deck
            };
        }
    }
}

