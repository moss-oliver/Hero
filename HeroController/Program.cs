using HeroCommon;
using System;
using System.IO;
using System.IO.Pipes;

namespace HeroController
{
    class Program
    {
        static void Main(string[] args)
        {
            var pipe = new NamedPipeClientStream(".", Connection.ConnectionString, PipeDirection.Out, PipeOptions.None);
            StreamWriter sw = new StreamWriter(pipe);
            pipe.Connect();
            
            //Init serializer for speed later.
            {
                PianoKeyPressEvent kpe = new PianoKeyPressEvent(new PianoKey(1, Note.A), true);
                kpe.Serialize();
            }

            var piano = new PianoController();
            piano.KeyChangeAction = (PianoKey key, bool down) =>
            {
                if (pipe.IsConnected)
                {
                    PianoKeyPressEvent kpe = new PianoKeyPressEvent(key, down);
                    sw.WriteLine(kpe.Serialize());
                    sw.Flush();
                    Console.Write(down ? "D: " : "U: ");
                    Console.WriteLine(kpe.Key.Note + " : " + kpe.Key.Octave);
                }
            };
            piano.Start();
            
            while (pipe.IsConnected)
            {
                System.Threading.Thread.Sleep(200);
                
            }
            Console.WriteLine("Done");
        }
    }
}
