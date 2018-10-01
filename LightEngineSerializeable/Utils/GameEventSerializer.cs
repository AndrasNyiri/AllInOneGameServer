using System.Collections.Generic;

namespace LightEngineSerializeable.Utils
{
    public class GameEventSerializer
    {
        public object[] Serialize(GameEvent gameEvent)
        {
            List<object> parameters = new List<object>();
            parameters.Add((int)gameEvent.Type);

            switch (gameEvent.Type)
            {
                case GameEventType.NetworkObjectSpawn:
                    var spawnEvent = (NetworkObjectSpawnEvent)gameEvent;
                    parameters.Add(spawnEvent.Id);
                    parameters.Add((int)spawnEvent.ObjectType);
                    parameters.Add(spawnEvent.PositionX);
                    parameters.Add(spawnEvent.PositionY);
                    break;
            }

            return parameters.ToArray();
        }

        public GameEvent Deserialize(params object[] parameters)
        {
            GameEvent gameEvent = null;
            //int paramIndex = 0;
            //GameEventType eventType = (GameEventType)parameters[paramIndex++];
            //switch (eventType)
            //{
            //    case GameEventType.NetworkObjectSpawn:
            //        gameEvent = new NetworkObjectSpawnEvent() { Id = (int)parameters[paramIndex++]
            //                                                   ,ObjectType = (NetworkObjectType)((int)parameters[paramIndex++])
            //                                                   ,PositionX = (float)
            //                                                   ,PositionY = };
            //        break;
            //}

            return gameEvent;
        }
    }
}
