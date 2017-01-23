using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Audio;

namespace Bot
{
    class LegdayBot : DiscordClient, IObservable<Message>{
        private List<IObserver<Message>> observers;
        private Dictionary<ulong, IAudioClient> ServerAudioClients;

        public LegdayBot()
        {
            observers = new List<IObserver<Message>>();
            Configure();
        }

        private void Configure()
        {
            //TODO
            GetService<CommandService>().CreateGroup("do", cgb =>
            {
                cgb.CreateCommand("join")
                .Description("Joins a audio channel")
                .Do(async e =>
                {
                    var audioClient = await GetService<AudioService>().Join(e.User.VoiceChannel ?? e.Server.VoiceChannels.FirstOrDefault());
                    ServerAudioClients.Add(e.Server.Id, audioClient);
                });

                //leave command
                cgb.CreateCommand("leave")
                .Description("leaves an audio channel")
                .Do(async e =>
                {
                    var serverId = e.User.Server.Id;
                    if (ServerAudioClients.ContainsKey(serverId))
                        ServerAudioClients.Remove(serverId);

                    await GetService<AudioService>().Leave(e.Server);
                });
            });

            ExecuteAndWait(async () =>
            {
                await Connect("MjYxODY1NzM2NjIyOTY0NzM2.Cz7qCQ.V5HrYRWrn2vsWiVX6-Jzs8w-4AQ", TokenType.Bot);
                while (true)
                {
                    //CommandEngine.Run(Console.ReadLine().Split(' '));
                    //Discord.Commands.Command b = new Discord.Commands.Command();
                    
                }
            });
        }

        #region IObservable implementation
        public IDisposable Subscribe(IObserver<Message> observer)
        {
            if (!observers.Contains(observer))
                   observers.Add(observer);

            return new Unsubscriber<Message>(observers, observer);
        }

        internal class Unsubscriber<Message> : IDisposable
        {
            private List<IObserver<Message>> _observers;
            private IObserver<Message> _observer;

            internal Unsubscriber(List<IObserver<Message>> observers, IObserver<Message> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
        #endregion
    }



}
