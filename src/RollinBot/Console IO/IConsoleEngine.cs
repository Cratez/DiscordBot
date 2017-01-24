using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Console_IO
{
    public interface IConsoleEngine
    {
    }

    public class ConsoleEngine
    {
        private List<IConsoleCommand> Commands;
        public Discord.DiscordClient Client;

        public ConsoleEngine(Discord.DiscordClient client)
        {
            Client = client;
            Commands = new List<IConsoleCommand>();
            //Commands.Add(new DefaultCommand { Action = () => { System.Console.WriteLine("Hello world"); }, Command = "HW" });
        }

        public void AddCommand(IConsoleCommand cmd)
        {
            cmd.Client = Client;
        }

        public void Run(bool KeepThread = false)
        {
            do
            {
                //parse input
                var input = Console.ReadLine().Split(' ').ToList();
                input.ForEach(s => s.ToLower());

                //grab command
                string cmd = input[0];

                //execute appropriate commands
                Commands.Where(c => c.Command.ToLower() == cmd).ToList().ForEach(c => c.Execute());
            } while (KeepThread);
        }
    }
}
