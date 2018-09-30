using LiteNetLib;

namespace LightGameServer.NetworkHandling.Handlers
{
    class CommandObjectHandler
    {
        public static CommandObjectHandler New(CommandObject commandObject, NetPeer peer)
        {
            return new CommandObjectHandler(commandObject, peer);
        }

        private readonly CommandObject _commandObject;
        private readonly NetPeer _peer;

        public CommandObjectHandler(CommandObject commandObject, NetPeer peer)
        {
            _commandObject = commandObject;
            _peer = peer;
        }

        public void HandleCommand()
        {
            switch (_commandObject.Command)
            {

            }
        }
    }
}
