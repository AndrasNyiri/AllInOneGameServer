using System;

namespace LightGameServer.ConsoleStuff
{
    public class MenuOption
    {
        public string DisplayText { get; set; }
        public Action Action { get; set; }
    }
}
