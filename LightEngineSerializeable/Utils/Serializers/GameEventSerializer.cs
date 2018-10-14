using System.Collections.Generic;
using LightEngineSerializeable.LiteNetLib.Utils;
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
                        break;
                    case GameEventType.PositionGroupSync:
                        var groupPositionSyncEvent = (PositionGroupSyncEvent)gameEvent;
                        parameters.Add(groupPositionSyncEvent.TimeStamp);
                        parameters.Add((byte)groupPositionSyncEvent.PositionSyncs.Length);
                        foreach (var posSync in groupPositionSyncEvent.PositionSyncs)
                        {
                            parameters.Add(posSync.Id);
                            parameters.Add(posSync.PositionX.ToShort());
                            parameters.Add(posSync.PositionY.ToShort());
                        }
                        break;
                    case GameEventType.GameStart:
                        var gameStartEvent = (GameStartEvent)gameEvent;
                        parameters.Add(gameStartEvent.NetworkTime);
                        parameters.Add((byte)gameStartEvent.PlayerType);
                        parameters.Add(gameStartEvent.LevelId);
                        parameters.Add(gameStartEvent.CanPlay);
                        parameters.Add((byte)gameStartEvent.SpawnEvents.Length);
                        foreach (var groupSpawnEvent in gameStartEvent.SpawnEvents)
                        {
                            parameters.Add(groupSpawnEvent.Id);
                            parameters.Add((byte)groupSpawnEvent.ObjectType);
                            parameters.Add(groupSpawnEvent.PositionX.ToShort());
                            parameters.Add(groupSpawnEvent.PositionY.ToShort());
                        }
                        break;
                    case GameEventType.TurnSync:
                        var turnSyncEvent = (TurnSyncEvent)gameEvent;
                        parameters.Add((byte)turnSyncEvent.PlayerType);
                        break;
                    case GameEventType.CanPlay:
                        var canPlayEvent = (CanPlayEvent)gameEvent;
                        parameters.Add(canPlayEvent.CanPlay);
                        parameters.Add(canPlayEvent.SelectedUnitId);
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
                        });
                        break;
                    case GameEventType.PositionGroupSync:
                        float groupTimeStamp = reader.GetFloat();
                        List<PositionSyncEvent> posSyncEvents = new List<PositionSyncEvent>();
                        byte syncCount = reader.GetByte();
                        for (int j = 0; j < syncCount; j++)
                        {
                            posSyncEvents.Add(new PositionSyncEvent
                            {
                                Id = reader.GetUShort(),
                                PositionX = reader.GetShort().ToFloat(),
                                PositionY = reader.GetShort().ToFloat()
                            });
                        }
                        eventList.Add(new PositionGroupSyncEvent
                        {
                            TimeStamp = groupTimeStamp,
                            PositionSyncs = posSyncEvents.ToArray()
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
                            spawnEvents.Add(new NetworkObjectSpawnEvent
                            {
                                Id = reader.GetUShort(),
                                ObjectType = (NetworkObjectType)reader.GetByte(),
                                PositionX = reader.GetShort().ToFloat(),
                                PositionY = reader.GetShort().ToFloat()
                            });
                        }
                        eventList.Add(new GameStartEvent
                        {
                            NetworkTime = networkTime,
                            PlayerType = playerType,
                            LevelId = levelId,
                            CanPlay = canPlay,
                            SpawnEvents = spawnEvents.ToArray()
                        });
                        break;
                    case GameEventType.TurnSync:
                        eventList.Add(new TurnSyncEvent
                        {
                            PlayerType = (PlayerType)reader.GetByte(),
                        });
                        break;
                    case GameEventType.CanPlay:
                        eventList.Add(new CanPlayEvent { CanPlay = reader.GetBool(), SelectedUnitId = reader.GetUShort() });
                        break;
                }
            }

            return eventList;
        }
    }
}
