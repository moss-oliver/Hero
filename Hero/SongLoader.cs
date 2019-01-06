using HeroCommon;
using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hero
{
    static class SongLoader
    {
        const double MicrosecondsPerMinute = 60000000;

        public static Song Load(string Filename)
        {
            double Bpm = 0;
            int BpmCount = 0;
            var noteList = new List<SongEntry>();
            MidiFile file = new MidiFile(Filename, false);
            foreach (var track in file.Events)
            {
                Console.WriteLine($"");
                Console.WriteLine($"---------------NEW TRACK-----------------");
                Console.WriteLine($"");
                foreach (var e in track)
                {
                    if (e.CommandCode == MidiCommandCode.MetaEvent)
                    {
                        var me = (MetaEvent)e;
                        if (me.MetaEventType == MetaEventType.SetTempo)
                        {
                            var mte = (TempoEvent)e;
                            Bpm += MicrosecondsPerMinute / ((double)mte.MicrosecondsPerQuarterNote);
                            BpmCount += 1;
                        }
                    }
                    else if (e.CommandCode == MidiCommandCode.PatchChange)
                    {
                        var pte = (PatchChangeEvent)e;
                    }
                    else if (e.CommandCode == MidiCommandCode.ControlChange)
                    {
                        var cce = (ControlChangeEvent)e;
                    }
                    else if (e.CommandCode == MidiCommandCode.NoteOn)
                    {
                        var noe = (NoteOnEvent)e;
                        if (noe.OffEvent != null)
                        {
                            string name = noe.NoteName;
                            PianoKey key = PianoKey.Parse(name);
                            if (key != null)
                            {
                                SongEntry entry = new SongEntry(key, noe.AbsoluteTime, noe.NoteLength);
                                noteList.Add(entry);
                            }
                        }
                    }
                    else
                    {
                        //var tse = (TimeSignatureEvent)e;
                    }
                }
            }

            if (BpmCount > 0) {
                Bpm = Bpm / BpmCount;
            }
            else
            {
                Bpm = 60;
            }

            return new Song(file.DeltaTicksPerQuarterNote, Bpm, noteList);
        }
    }
}
