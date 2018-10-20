using System.Collections.Generic;
using LightEngineSerializeable.LiteNetLib.Utils;
using LightEngineSerializeable.SerializableClasses.DatabaseModel;
using LightEngineSerializeable.SerializableClasses.Enums;
using LightEngineSerializeable.SerializableClasses.GameModel;
using LightEngineSerializeable.SerializableClasses.GameModel.RequestEvents;

namespace LightEngineSerializeable.Utils.Serializers
{
    public class RequestEventSerializer
    {
        public NetDataWriter Serialize(params RequestEvent[] requests)
        {
            var parameters = new List<object> { (byte)NetworkCommand.RequestEventOption, (byte)requests.Length };

            foreach (var request in requests)
            {
                parameters.Add((byte)request.Type);
                parameters.AddRange(request.GetPropertyValues());
            }

            return ObjectSerializationUtil.SerializeObjects(parameters.ToArray());
        }

        public List<RequestEvent> Deserialize(NetDataReader reader)
        {
            List<RequestEvent> requestList = new List<RequestEvent>();
            int count = reader.GetByte();

            for (int i = 0; i < count; i++)
            {
                RequestEventType requestType = (RequestEventType)reader.GetByte();
                switch (requestType)
                {
                    case RequestEventType.PlayUnitAbility:
                        requestList.Add(SerializableModel.DeSerialize<PlayUnitAbilityRequest>(reader));
                        break;
                    case RequestEventType.SetAimDirection:
                        requestList.Add(SerializableModel.DeSerialize<SetAimDirectionRequest>(reader));
                        break;
                }
            }

            return requestList;
        }
    }
}
