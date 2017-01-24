using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Observers.ChatObservers
{
    class TTSListener : IObserver<Message>
    {
        List<uint> TTSMembers = new List<uint>();

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            //Do nothing
        }

        public void OnNext(Message value)
        {
            //check our message for tts stuffs
        }

        void AddTTSMember(ulong member)
        {

        }

        void RemoveTTSMember(ulong member)
        {

        }
    }
}
