using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace LightEngineSerializeable.Utils
{
    public class ObjectSerializationUtil
    {
        public static IEnumerable<List<T>> SplitList<T>(List<T> locations, int nSize = 30)
        {
            for (int i = 0; i < locations.Count; i += nSize)
            {
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
            }
        }

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

        public static List<CommandObject> CreateCommandObjectsFromByteArray(byte[] buffer)
        {
            List<CommandObject> cmdList = new List<CommandObject>();
            bool done = false;
            while (!done)
            {
                int size = BitConverter.ToInt32(buffer, 0);
                int currentReceived = size + 4;
                var cmdBuffer = new byte[size];
                Array.Copy(buffer, 4, cmdBuffer, 0, size);

                CommandObject rCmd = (CommandObject)ByteArrayToObject(cmdBuffer);
                cmdList.Add(rCmd);

                if (currentReceived < buffer.Length)
                {
                    byte[] temp = new byte[buffer.Length - currentReceived];
                    Array.Copy(buffer, currentReceived, temp, 0, temp.Length);
                    buffer = temp;
                }
                else
                {
                    done = true;
                }
            }

            return cmdList;
        }

        public static byte[] CreateCmdWithPrefix(CommandObject cmd)
        {
            var commandBytes = ObjectToByteArray(cmd);
            int length = commandBytes.Length;
            List<byte> sendList = new List<byte>();
            sendList.AddRange(BitConverter.GetBytes(length));
            sendList.AddRange(commandBytes);
            return sendList.ToArray();
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
