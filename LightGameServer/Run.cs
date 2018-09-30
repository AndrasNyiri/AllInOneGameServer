using LightGameServer.NetworkHandling;
using NLog;

namespace LightGameServer
{
    class Run
    {
        static void Main(string[] args)
        {
            Server.Get().Start();
            LogManager.Shutdown();
        }
    }
}
