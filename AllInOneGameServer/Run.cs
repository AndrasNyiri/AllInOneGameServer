using System;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;

namespace AllInOneGameServer
{
    class Run
    {
        private const string CONNECTION_KEY = "PzM@.5p&k!aZJXH6,mq44R\\ue?%BSSS*t\'N8xxH=L+\"S\'4^N,m5M{`N;>K]7{vUB[R!B\"?>sV!&d~b(G-pYW%5&,6_J5>Hky95.DTG_dhM^x]ph(&.\\.Xc(B.fFGW`e_";

        static void Main(string[] args)
        {
            EventBasedNetListener listener = new EventBasedNetListener();
            NetManager server = new NetManager(listener, 10000, CONNECTION_KEY);
            server.Start(60001);
            Console.WriteLine("Server started...");


            listener.PeerConnectedEvent += peer =>
            {
                Console.WriteLine("We got connection: {0}", peer.EndPoint);
                NetDataWriter writer = new NetDataWriter();
                writer.Put("Hello client!");
                peer.Send(writer, SendOptions.ReliableOrdered);
            };


            while (!Console.KeyAvailable)
            {
                server.PollEvents();
                Thread.Sleep(15);
            }
            server.Stop();
        }
    }
}
