﻿using System.Collections.Generic;
using LiteNetLib.Utils;

namespace LightEngineSerializeable.Utils
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
                    case RequestEventType.PushGameObject:
                        var pushRequest = (PushGameObjectRequest)request;
                        parameters.Add(pushRequest.GameObjectId);
                        parameters.Add(pushRequest.DirectionX);
                        parameters.Add(pushRequest.DirectionY);
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
                    case RequestEventType.PushGameObject:
                        requestList.Add(new PushGameObjectRequest
                        {
                            GameObjectId = reader.GetUShort(),
                            DirectionX = reader.GetFloat(),
                            DirectionY = reader.GetFloat()
                        });
                        break;
                }
            }

            return requestList;
        }
    }
}
