using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Audio;
using System.IO;
using Newtonsoft.Json;


namespace Bot
{
    class ChannelBackup
    {

    }

    public class LegdayBot : DiscordClient, IObservable<Message> {
        private List<IObserver<Message>> observers;
        private Dictionary<ulong, IAudioClient> ServerAudioClients;

        public LegdayBot()
        {
            observers = new List<IObserver<Message>>();
            Configure();

        }

        private void Configure()
        {
            AddService<CommandService>();
            MessageReceived += (object o, MessageEventArgs e) =>
            {
                Server s = e.Server;
                var permissions = s.AllChannels.FirstOrDefault();


                File.WriteAllText("data.json", JsonConvert.SerializeObject(permissions.PermissionOverwrites));
                var pover = permissions.PermissionOverwrites.FirstOrDefault();
                //Channel.PermissionOverwrite t = new Channel.PermissionOverwrite


                var backup = JsonConvert.DeserializeObject<Discord.Channel.PermissionOverwrite>(File.ReadAllText("data.json"));
                //permissions.AddPermissionsRule()
                //File.WriteAllText("data.json", JsonConvert.SerializeObject(channels));
                //File.WriteAllText("data.json",JsonConvert.SerializeObject(s).ToString());
            };

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
                await Connect(
                    //Happy now you dumbass token stealer?
                    File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).ToString() , "discordbot.tok")), 
                    TokenType.Bot);

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
