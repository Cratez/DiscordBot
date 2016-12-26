using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class DefaultCommand : AConsoleCommand
    {
        public Action Action { get; set; } = null;

        public override bool Execute()
        {
            Action();
            return true;
        }
    }

    public class SayCommand : AConsoleCommand
    {
        public SayCommand()
        {
            Command = "say";
        }

        public override bool Execute()
        {
            var result = Client.Servers.Where(c => c.Name == "Cratez")?.FirstOrDefault().DefaultChannel.SendMessage("Hello world!").Result;
            return true;
        }
    }

    public class QuitCommand : AConsoleCommand
    {
        public QuitCommand()
        {
            Command = "quit";
        }

        public override bool Execute()
        {
            Client.Disconnect().Wait();
            Environment.Exit(0);
            return true;//foobar, this never executes
        }
    }
}
