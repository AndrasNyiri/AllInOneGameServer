using System;
using System.Collections.Generic;
using System.Linq;
using LiteNetLib;
using LiteNetLib.Utils;

namespace LightEngineSerializeable.Utils
{
    public class DataSender
    {
        public class TypeSwitch
        {
            private readonly Dictionary<Type, Action<object>> _matches = new Dictionary<Type, Action<object>>();
            public TypeSwitch Case<T>(Action<T> action) { _matches.Add(typeof(T), x => action((T)x)); return this; }
            public void Switch(object x) { _matches[x.GetType()](x); }
        }


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

        public void SendCommandObject(CommandObject commandObject, SendOptions sendOption = SendOptions.ReliableOrdered)
        {
            try
            {
                NetDataWriter writer = new NetDataWriter(true);
                writer.Put((byte)NetworkCommand.CommandObjectOption);
                var commandBytes = ObjectSerializationUtil.ObjectToByteArray(commandObject);
                writer.PutBytesWithLength(commandBytes);
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
