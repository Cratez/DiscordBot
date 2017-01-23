using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Console_IO
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Does not implement GetHashCode", "CS0661")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Does not implement Equals(object o)", "CS0660")]
    public abstract class AConsoleCommand : IConsoleCommand, IEquatable<string>
    {
        public string Command { get; set; } = "Default";
        public DiscordClient Client { get; set; } = null;

        public AConsoleCommand(string command, DiscordClient client)
        {

        }

        public bool Equals(string str)
        {
            if (Command?.ToLower() == str?.ToLower())
                return true;

            return false;
        }

        public static bool operator ==(AConsoleCommand cmd, string str)
       {
            return object.ReferenceEquals(cmd.Command,str) ||
                !object.ReferenceEquals(cmd, null) && cmd.Equals(str);
        }

        public static bool operator !=(AConsoleCommand cmd, string str)
        {
            return !(cmd == str);
        }

        public abstract bool Execute();
    }

    public class DefaultCommand : AConsoleCommand
    {
        public Action Action { get; set; } = null;

        public override bool Execute()
        {
            try {
                if (Action != null)
                    Action();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public DefaultCommand(string cmd, DiscordClient client) : base(cmd, client)
        {

        }
    }
}
