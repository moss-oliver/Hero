using System;
using System.Collections.Generic;
using NAudio.Midi;
using System.Threading.Tasks;
using System.Linq;
using HeroCommon;
using System.IO.Pipes;
using System.IO;
using System.Collections.Concurrent;

namespace Hero
{
    class Program
    {
        static void Main(string[] args)
        {
            Song song = SongLoader.Load("..\\..\\..\\..\\songs\\moonlight_sonata.mid");
            
            Console.WriteLine("Starting Server");

            ConcurrentDictionary<PianoKey, byte> pressedKeys = new ConcurrentDictionary<PianoKey, byte>();

            var pianoPlayer = new PianoPlayer();

            Task.Run(() =>
            {
                var pipe = new NamedPipeServerStream(Connection.ConnectionString, PipeDirection.In, 1);
                StreamReader sr = new StreamReader(pipe);
                Console.WriteLine("Waiting for connection....");
                
                pipe.WaitForConnection();
                
                //Init deserializer for speed later.
                {
                    PianoKeyPressEvent.Deserialize(new PianoKeyPressEvent(new PianoKey(1, Note.A), true).Serialize());
                }
                Console.WriteLine("Connected");

                while (pipe.IsConnected)
                {
                    var kpe = PianoKeyPressEvent.Deserialize(sr.ReadLine());
                    if(kpe.Pressed)
                    {
                        pressedKeys.AddOrUpdate(kpe.Key, 0, (x,y) => 0);
                    }
                    else
                    {
                        pressedKeys.Remove(kpe.Key, out byte x);
                    }
                }
                Console.WriteLine("Connection closed.");
            });
            
            using (DisplayWindow visualizer = new DisplayWindow(512, 512, "Piano"))
            {
                visualizer.SoundPlayer = pianoPlayer;
                visualizer.CurrentSong = song;
                visualizer.PressedKeys = pressedKeys;
                visualizer.Run(30, 30);
            }
            
            Console.ReadKey();
        }
        
    }
}
