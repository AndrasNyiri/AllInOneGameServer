using System;
using System.Collections.Generic;
using LightEngineSerializeable.Utils;
using LiteNetLib;
using LiteNetLib.Utils;

namespace LightGameServer.NetworkHandling.Handlers
{
    class DataSender
    {
        public class TypeSwitch
        {
            private readonly Dictionary<Type, Action<object>> _matches = new Dictionary<Type, Action<object>>();
            public TypeSwitch Case<T>(Action<T> action) { _matches.Add(typeof(T), (x) => action((T)x)); return this; }
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

        public void Send(NetworkCommand command, SendOptions sendOption = SendOptions.ReliableOrdered, params object[] datas)
        {
            try
            {
                NetDataWriter writer = new NetDataWriter(true);
                writer.Put((byte)command);
                var ts = new TypeSwitch()
                    .Case((string x) => { writer.Put(x); })
                    .Case((int x) => { writer.Put(x); })
                    .Case((float x) => { writer.Put(x); })
                    .Case((long x) => { writer.Put(x); })
                    .Case((ulong x) => { writer.Put(x); })
                    .Case((byte[] x) => { writer.PutBytesWithLength(x); });

                foreach (var data in datas)
                {
                    ts.Switch(data);
                }

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
