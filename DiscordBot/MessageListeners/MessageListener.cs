using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    using Discord;

    public class MessageEventArgs : EventArgs
    {
        public Message Message { get; set; } = null;

        public MessageEventArgs() { }
        public MessageEventArgs(Message message) { Message = message; }
    }

    public delegate void MessageEventHandler(object sender, MessageEventArgs e);
    public abstract class MessageListener
    {
        //public event 
        public MessageListener(){}

        public abstract void OnMessage(object caller, Message message);
    }

    public class TTSListener : MessageListener
    {
        public override void OnMessage(object caller, Message message)
        {
            throw new NotImplementedException();
        }
    }
}
