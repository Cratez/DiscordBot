using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    class ConsoleCommands
    {
        private Program Console;

        private List<AConsoleCommand> Commands;

        public ConsoleCommands(Program parent)
        {
            Console = parent;
            Commands = new List<AConsoleCommand>();
            Commands.Add(new SayCommand { Client = parent.Client, });
            Commands.Add(new QuitCommand { Client = parent.Client });
            Commands.Add(new DefaultCommand { Action = () => { System.Console.WriteLine("Hello world"); }, Command = "HW" });
        }

        public void AddCommand(IConsoleCommand cmd)
        {
            cmd.Client = Console.Client;
        }

        public void Run(string[] args)
        {
            string cmd = args[0].ToLower();

            Commands.Where(c => c.Command.ToLower() == cmd).ToList().ForEach(c => c.Execute());
        }
    }
}
