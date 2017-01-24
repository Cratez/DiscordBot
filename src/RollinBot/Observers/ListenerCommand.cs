using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Observers
{
    interface ListenerCommand
    {
        string Command { get; set; }
        string Group { get; set; }
    }
}
