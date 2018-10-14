using System;
using System.Linq;
using LightEngineSerializeable.LiteNetLib;
using LightEngineSerializeable.LiteNetLib.Utils;
using LightEngineSerializeable.SerializableClasses.Enums;
using LightEngineSerializeable.Utils.Serializers;

namespace LightEngineSerializeable.Utils
{
    public class DataSender
    {
        public static DataSender New(NetPeer peer)
        {
            return new DataSender(peer);
        }

        private readonly NetPeer _peer;

        public DataSender(NetPeer peer)
        {
            _peer = peer;
        }

        public void Send(NetworkCommand command, SendOptions sendOption, params object[] data)
        {
            try
            {
                var sendData = data.ToList();
                sendData.Insert(0, (byte)command);
                _peer.Send(ObjectSerializationUtil.SerializeObjects(sendData.ToArray()), sendOption);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        public void Send(NetDataWriter writer, SendOptions sendOption)
        {
            try
            {
                _peer.Send(writer, sendOption);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

    }
}
