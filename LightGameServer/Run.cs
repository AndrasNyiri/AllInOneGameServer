using LightEngineSerializeable.LiteNetLib.Utils;
using LightEngineSerializeable.SerializableClasses;
using LightEngineSerializeable.SerializableClasses.Enums;
using LightGameServer.Database;
using LightGameServer.NetworkHandling;
using NLog;

namespace LightGameServer
{
    class Run
    {
        static void Main(string[] args)
        {
            QueryRepository repo = new QueryRepository();
            DataStore store = new DataStore(repo);
            var sd = store.Data.Serialize(NetworkCommand.GetStaticData);

            StaticData sdata = StaticData.DeSerialize(new NetDataReader(sd.Data), true);

            //Server.Get().Start();
            //LogManager.Shutdown();
        }
    }
}
