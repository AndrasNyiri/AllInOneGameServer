using System.Collections.Generic;
using LightEngineSerializeable.LiteNetLib.Utils;
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
                switch (request.Type)
                {
                    case RequestEventType.PlayUnitAbility:
                        var pushRequest = (PlayUnitAbilityRequest)request;
                        parameters.Add(pushRequest.DirectionX);
                        parameters.Add(pushRequest.DirectionY);
                        break;
                    case RequestEventType.SetAimDirection:
                        var aimRequest = (SetAimDirectionRequest)request;
                        parameters.Add(aimRequest.Active);
                        parameters.Add(aimRequest.DirectionX);
                        parameters.Add(aimRequest.DirectionZ);
                        break;
                }
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
                        requestList.Add(new PlayUnitAbilityRequest
                        {
                            DirectionX = reader.GetFloat(),
                            DirectionY = reader.GetFloat()
                        });
                        break;
                    case RequestEventType.SetAimDirection:
                        requestList.Add(new SetAimDirectionRequest
                        {
                            Active = reader.GetBool(),
                            DirectionX = reader.GetFloat(),
                            DirectionZ = reader.GetFloat()
                        });
                        break;
                }
            }

            return requestList;
        }
    }
}
