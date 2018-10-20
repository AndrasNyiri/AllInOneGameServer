using System.Collections.Generic;
using LightEngineSerializeable.LiteNetLib.Utils;
using LightEngineSerializeable.SerializableClasses.DatabaseModel;
using LightEngineSerializeable.SerializableClasses.Enums;
using LightEngineSerializeable.SerializableClasses.GameModel;
using LightEngineSerializeable.SerializableClasses.GameModel.GameEvents;

namespace LightEngineSerializeable.Utils.Serializers
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
                    case GameEventType.PositionGroupSync:
                        var groupPositionSyncEvent = (PositionGroupSyncEvent)gameEvent;
                        parameters.Add(groupPositionSyncEvent.TimeStamp);
                        parameters.Add((byte)groupPositionSyncEvent.PositionSyncs.Count);
                        foreach (var posSync in groupPositionSyncEvent.PositionSyncs)
                        {
                            parameters.AddRange(posSync.GetPropertyValues());
                        }
                        break;
                    case GameEventType.GameStart:
                        var gameStartEvent = (GameStartEvent)gameEvent;
                        parameters.Add(gameStartEvent.TimeStamp);
                        parameters.Add((byte)gameStartEvent.PlayerType);
                        parameters.Add(gameStartEvent.LevelId);
                        parameters.Add(gameStartEvent.CanPlay);
                        parameters.Add((byte)gameStartEvent.SpawnEvents.Length);
                        foreach (var groupSpawnEvent in gameStartEvent.SpawnEvents)
                        {
                            parameters.AddRange(groupSpawnEvent.GetPropertyValues());
                        }
                        break;
                    default:
                        parameters.AddRange(gameEvent.GetPropertyValues());
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
                        eventList.Add(SerializableModel.DeSerialize<NetworkObjectSpawnEvent>(reader));
                        break;
                    //case GameEventType.PositionSync:
                    //    eventList.Add(SerializableModel.DeSerialize<PositionSyncEvent>(reader));
                    //    break;
                    case GameEventType.PositionGroupSync:
                        float groupTimeStamp = reader.GetFloat();
                        List<PositionSyncEvent> posSyncEvents = new List<PositionSyncEvent>();
                        byte posCount = reader.GetByte();
                        for (int j = 0; j < posCount; j++)
                        {
                            posSyncEvents.Add(SerializableModel.DeSerialize<PositionSyncEvent>(reader));
                        }
                        eventList.Add(new PositionGroupSyncEvent
                        {
                            TimeStamp = groupTimeStamp,
                            PositionSyncs = posSyncEvents
                        });
                        break;
                    case GameEventType.GameStart:
                        float networkTime = reader.GetFloat();
                        var playerType = (PlayerType)reader.GetByte();
                        var levelId = reader.GetByte();
                        var canPlay = reader.GetBool();
                        List<NetworkObjectSpawnEvent> spawnEvents = new List<NetworkObjectSpawnEvent>();
                        byte spawnCount = reader.GetByte();
                        for (int j = 0; j < spawnCount; j++)
                        {
                            spawnEvents.Add(SerializableModel.DeSerialize<NetworkObjectSpawnEvent>(reader));
                        }
                        eventList.Add(new GameStartEvent
                        {
                            TimeStamp = networkTime,
                            PlayerType = playerType,
                            LevelId = levelId,
                            CanPlay = canPlay,
                            SpawnEvents = spawnEvents.ToArray()
                        });
                        break;
                    case GameEventType.TurnSync:
                        eventList.Add(SerializableModel.DeSerialize<TurnSyncEvent>(reader));
                        break;
                    case GameEventType.CanPlay:
                        eventList.Add(SerializableModel.DeSerialize<CanPlayEvent>(reader));
                        break;
                    case GameEventType.NetworkObjectDestroy:
                        eventList.Add(SerializableModel.DeSerialize<NetworkObjectDestroyEvent>(reader));
                        break;
                    case GameEventType.UnitHealthSync:
                        eventList.Add(SerializableModel.DeSerialize<UnitHealthSyncEvent>(reader));
                        break;
                }
            }

            return eventList;
        }
    }
}
