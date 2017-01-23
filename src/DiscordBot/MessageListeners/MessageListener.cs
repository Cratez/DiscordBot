using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    using Discord;

    public abstract class MessageListener : IObserver<Message>
    {
        private IDisposable unsubscriber;
        private Message last;


        public void OnCompleted()
        {
            //free resources?
        }

        public void OnError(Exception error)
        {
            //do nothing
        }

        public abstract void OnNext(Message message);

        public virtual void Unsubscribe()
        {
            unsubscriber.Dispose();
        }
    }

    public class TTSListener : MessageListener
    {
        public override void OnNext(Message message)
        {
            throw new NotImplementedException();
        }
    }
}
