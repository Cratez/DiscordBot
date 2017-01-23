using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech;
using System.Speech.Synthesis;


namespace DiscordBot
{
    using Discord;
    using Discord.Audio;
    using Discord.Commands;
    using NAudio.Wave;
    using System.IO;
    using System.Threading;

    //Client ID: 261865736622964736
    //Client Secret: 0Tg-T3uSJnYDRQdsQ_WL1rEvwMKbpQqA
    public class Program : IObservable<Message>
    {
        public DiscordClient Client;
        private ConsoleCommands CommandEngine;
        private Dictionary<ulong, IAudioClient> ServerAudioClients;
        private HashSet<ulong> TTSMembers;
        private List<IObserver<Message>> observers;

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<Message>> _observers;
            private IObserver<Message> _observer;

            public Unsubscriber(List<IObserver<Message>> observers, IObserver<Message> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (!(_observer == null)) _observers.Remove(_observer);
            }
        }

        public IDisposable Subscribe(IObserver<Message> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);

            return new Unsubscriber(observers, observer);
        }

        static void Main(string[] args) => new Program().Start(args);
 
        private void Start(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
            observers = new List<IObserver<Message>>();
            Client = new DiscordClient();
            CommandEngine = new ConsoleCommands(this);
            ServerAudioClients = new Dictionary<ulong, IAudioClient>();
            TTSMembers = new HashSet<ulong>();

            //Configure client
            Configure();

            //start client and loop
            Client.ExecuteAndWait(async () =>
            {
                await Client.Connect("MjYxODY1NzM2NjIyOTY0NzM2.Cz7qCQ.V5HrYRWrn2vsWiVX6-Jzs8w-4AQ", TokenType.Bot);
                while (true)
                    CommandEngine.Run(Console.ReadLine().Split(' '));
            });
        }

        //client configuration methods -- commands, etc...
        private void Configure()
        {
            Client.UsingAudio(x => x.Mode = AudioMode.Outgoing);
            Client.UsingCommands(x =>
            {
                x.PrefixChar = '-';
                x.HelpMode = HelpMode.Public;
            });

            Client.MessageReceived += (s, e) =>
            {
                if (TTSMembers.Contains(e.User.Id))
                {
                    new Thread(() =>
                    {
                        PlayTTS(e.Message, ServerAudioClients[e.Server.Id]);
                    }).Start();
                }
            };

            Client.GetService<CommandService>().CreateGroup("do", cgb =>
            {
                //greet command sample
                cgb.CreateCommand("greet")
                    .Alias(new string[] { "gr", "hi" })
                    .Description("Greets a person.")
                    .Parameter("GreetedPerson", ParameterType.Required)
                    .Do(async e =>
                    {
                        await e.Channel.SendMessage($"{e.User.Name} greets {e.GetArg("GreetedPerson")}");
                    });

                //bye command sample
                cgb.CreateCommand("bye")
                        .Alias(new string[] { "bb", "gb" })
                        .Description("Greets a person.")
                        .Parameter("GreetedPerson", ParameterType.Required)
                        .Do(async e =>
                        {
                            await e.Channel.SendMessage($"{e.User.Name} says goodbye to {e.GetArg("GreetedPerson")}");
                        });

                //join command
                cgb.CreateCommand("join")
                .Description("Joins a audio channel")
                .Do(async e =>
                {
                    var audioClient = await Client.GetService<AudioService>().Join(e.User.VoiceChannel ?? e.Server.VoiceChannels.FirstOrDefault());
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

                    await Client.GetService<AudioService>().Leave(e.Server);
                });

                cgb.CreateCommand("tts")
                .Description("Signs up a user for tts")
                .Do(e =>
               {
                   if (TTSMembers.Contains(e.User.Id))
                       TTSMembers.Remove(e.User.Id);
                   else
                       TTSMembers.Add(e.User.Id);
               });
            });

            Client.GetService<CommandService>().CreateCommand("say")
                .Description("Speaks the following audio to tts")
                .Do(e =>
                {
                    //PlayTTS(e.Message, ServerAudioClients[e.Server.Id]);
                    SendAudio(@"C: \Users\John\Music\Leroy Jenkins Sound Clip.mp3", ServerAudioClients[e.Server.Id]);
                });
        }

        object TTSLock = new object();

        private void PlayTTS(Message message, IAudioClient aclient)
        {
            lock (TTSLock)
            {
                var waveOut = new WaveOut();
                using (var synth = new SpeechSynthesizer())
                {
                    using (var stream = new MemoryStream())
                    {
                        synth.SelectVoiceByHints(VoiceGender.Male);
                        synth.SetOutputToWaveStream(stream);
                        synth.Speak(message.ToString());

                        stream.Seek(0, SeekOrigin.Begin);
                        var OutFormat = new WaveFormat(48000, 16, Client.GetService<AudioService>().Config.Channels);
                        using (var resampler = new MediaFoundationResampler(new RawSourceWaveStream(stream, new WaveFormat(12000, 16, Client.GetService<AudioService>().Config.Channels)), OutFormat))
                        {
                            resampler.ResamplerQuality = 60; // Set the quality of the resampler to 60, the highest quality
                            int blockSize = OutFormat.AverageBytesPerSecond / 50; // Establish the size of our AudioBuffer
                            byte[] buffer = new byte[blockSize];
                            int byteCount;

                            while ((byteCount = resampler.Read(buffer, 0, blockSize)) > 0) // Read audio into our buffer, and keep a loop open while data is present
                            {
                                if (byteCount < blockSize)
                                {
                                    // Incomplete Frame
                                    for (int i = byteCount; i < blockSize; i++)
                                        buffer[i] = 0;
                                }
                                aclient.Send(buffer, 0, blockSize); // Send the buffer to Discord
                            }
                        }

                        //stream.Seek(0, SeekOrigin.Begin);
                        //IWaveProvider provider = new RawSourceWaveStream(stream, new WaveFormat(48000, 16, Client.GetService<AudioService>().Config.Channels));

                        //waveOut.Init(provider);
                        //waveOut.Play();
                        //while (waveOut.PlaybackState == PlaybackState.Playing) ;

                    }
                }
            }
        }

        public void SendAudio(string filePath, IAudioClient aclient)
        {
            var channelCount = Client.GetService<AudioService>().Config.Channels; // Get the number of AudioChannels our AudioService has been configured to use.
            var OutFormat = new WaveFormat(48000, 16, channelCount); // Create a new Output Format, using the spec that Discord will accept, and with the number of channels that our client supports.
            using (var MP3Reader = new Mp3FileReader(filePath)) // Create a new Disposable MP3FileReader, to read audio from the filePath parameter
            using (var resampler = new MediaFoundationResampler(MP3Reader, OutFormat)) // Create a Disposable Resampler, which will convert the read MP3 data to PCM, using our Output Format
            {
                resampler.ResamplerQuality = 60; // Set the quality of the resampler to 60, the highest quality
                int blockSize = OutFormat.AverageBytesPerSecond / 50; // Establish the size of our AudioBuffer
                byte[] buffer = new byte[blockSize];
                int byteCount;

                while ((byteCount = resampler.Read(buffer, 0, blockSize)) > 0) // Read audio into our buffer, and keep a loop open while data is present
                {
                    if (byteCount < blockSize)
                    {
                        // Incomplete Frame
                        for (int i = byteCount; i < blockSize; i++)
                            buffer[i] = 0;
                    }
                    aclient.Send(buffer, 0, blockSize); // Send the buffer to Discord
                }
            }

        }

        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject.ToString());
            Console.WriteLine("Press Enter to continue");
            Console.ReadLine();
            Environment.Exit(1);
        }
    }
}
