using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightGameServer.ConsoleStuff
{
    public class ConsoleMenu
    {
        private readonly List<MenuOption> _options;

        public ConsoleMenu(params MenuOption[] options)
        {
            _options = options.ToList();
        }

        public ConsoleMenu Add(string displayText, Action action)
        {
            _options.Add(new MenuOption { DisplayText = displayText, Action = action });
            return this;
        }

        public void Update()
        {
            if (Console.KeyAvailable)
            {
                var input = Console.ReadKey(true);
                if (char.IsDigit(input.KeyChar))
                {
                    var result = int.Parse(input.KeyChar.ToString());
                    int index = result - 1;
                    if (index >= 0 && index < _options.Count)
                    {
                        _options[index].Action();
                        Display();
                    }
                }
            }
        }

        public void Display()
        {
            Console.Clear();
            StringBuilder menuBuilder = new StringBuilder();
            for (var i = 0; i < _options.Count; i++)
            {
                var menuOption = _options[i];
                menuBuilder.AppendLine(string.Format("{0}. {1}", i + 1, menuOption.DisplayText));
            }
            Console.WriteLine(menuBuilder.ToString());
        }

    }
}
