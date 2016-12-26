using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    /// <summary>
    /// Interface for console commands
    /// </summary>
    public interface IConsoleCommand
    {
        string Command { get; set; }

        DiscordClient Client { get; set; }

        bool Execute();
    }

    //Abstract command to contain base info, to save redundant declaration
    public abstract class AConsoleCommand : IConsoleCommand
    {
        public string Command { get; set; } = "Default";
        public DiscordClient Client { get; set; } = null;

        public abstract bool Execute();
    }
}
