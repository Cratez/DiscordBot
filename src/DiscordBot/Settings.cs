using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    //sudo singleton class for settings
    public class Settings
    {
        private static Settings settings = new Settings();

        public static Settings Instance { get; }
    }
}
