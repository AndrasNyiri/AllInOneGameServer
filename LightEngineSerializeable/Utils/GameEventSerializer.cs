using System.Collections.Generic;
using LiteNetLib.Utils;

namespace LightEngineSerializeable.Utils
{
    public class GameEventSerializer
    {
        public NetDataWriter Serialize(params GameEvent[] gameEvents)
        {
            var parameters = new List<object> { (byte)NetworkCommand.GameEventOption, (byte)gameEvents.Length };

            foreach (var gameEvent in gameEvents)
            {
                parameters.Add((byte)gameEvent.Type);
                switch (gameEvent.Type)
                {
                    case GameEventType.NetworkObjectSpawn:
                        var spawnEvent = (NetworkObjectSpawnEvent)gameEvent;
                        parameters.Add(spawnEvent.Id);
                        parameters.Add((byte)spawnEvent.ObjectType);
                        parameters.Add(spawnEvent.PositionX.ToShort());
                        parameters.Add(spawnEvent.PositionY.ToShort());
                        break;
                    case GameEventType.PositionSync:
                        var positionSyncEvent = (PositionSyncEvent)gameEvent;
                        parameters.Add(positionSyncEvent.Id);
                        parameters.Add(positionSyncEvent.PositionX.ToShort());
                        parameters.Add(positionSyncEvent.PositionY.ToShort());
                        parameters.Add(positionSyncEvent.TimeStamp);
                        break;
                }
            }

            return ObjectSerializationUtil.SerializeObjects(parameters.ToArray());
        }

        public List<GameEvent> Deserialize(NetDataReader reader)
        {
            List<GameEvent> eventList = new List<GameEvent>();
            int count = reader.GetByte();

            for (int i = 0; i < count; i++)
            {
                GameEventType eventType = (GameEventType)reader.GetByte();
                switch (eventType)
                {
                    case GameEventType.NetworkObjectSpawn:
                        eventList.Add(new NetworkObjectSpawnEvent()
                        {
                            Id = reader.GetUShort(),
                            ObjectType = (NetworkObjectType)reader.GetByte(),
                            PositionX = reader.GetShort().ToFloat(),
                            PositionY = reader.GetShort().ToFloat()
                        });
                        break;
                    case GameEventType.PositionSync:
                        eventList.Add(new PositionSyncEvent()
                        {
                            Id = reader.GetUShort(),
                            PositionX = reader.GetShort().ToFloat(),
                            PositionY = reader.GetShort().ToFloat(),
                            TimeStamp = reader.GetFloat()
                        });
                        break;
                }
            }

            return eventList;
        }
    }
}
