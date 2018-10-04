using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using LiteNetLib.Utils;

namespace LightEngineSerializeable.Utils
{
    public class ObjectSerializationUtil
    {


        public static byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        public static Object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            binForm.Binder = new PreMergeToMergedDeserializationBinder();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = binForm.Deserialize(memStream);

            return obj;
        }

        public static CommandObject CreateCommandObjectFromByteArray(byte[] commandBytes)
        {
            return (CommandObject)ByteArrayToObject(commandBytes);
        }


        public static NetDataWriter SerializeObjects(params object[] parameters)
        {
            NetDataWriter writer = new NetDataWriter(true);
            var ts = new DataSender.TypeSwitch()
                .Case((string x) => { writer.Put(x); })
                .Case((int x) => { writer.Put(x); })
                .Case((float x) => { writer.Put(x); })
                .Case((long x) => { writer.Put(x); })
                .Case((ulong x) => { writer.Put(x); })
                .Case((byte x) => { writer.Put(x); })
                .Case((short x) => { writer.Put(x); })
                .Case((ushort x) => { writer.Put(x); })
                .Case((double x) => { writer.Put(x); })
                .Case((bool x) => { writer.Put(x); })
                .Case((byte[] x) => { writer.PutBytesWithLength(x); });

            foreach (var data in parameters)
            {
                ts.Switch(data);
            }

            return writer;
        }


        sealed class PreMergeToMergedDeserializationBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                Type typeToDeserialize = null;
                String exeAssembly = Assembly.GetExecutingAssembly().FullName;
                typeToDeserialize = Type.GetType(String.Format("{0}, {1}",
                    typeName, exeAssembly));
                return typeToDeserialize;
            }
        }
    }
}
