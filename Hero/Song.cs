using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using HeroCommon;

namespace Hero
{
    class SongEntry
    {
        PianoKey key;
        public PianoKey Key
        {
            get => key;
        }

        long eventStart;
        public long EventStart
        {
            get => eventStart;
        }

        long eventDuration;
        public long EventDuration
        {
            get => eventDuration;
        }

        public SongEntry(PianoKey Key, long EventStart, long EventDuration)
        {
            this.key = Key;
            this.eventStart = EventStart;
            this.eventDuration = EventDuration;
        }
    }

    class Song
    {
        int ticksPerQuarterNote;
        public int TicksPerQuarterNote
        {
            get
            {
                return ticksPerQuarterNote;
            }
        }

        double bpm = 0;
        public double Bpm
        {
            get
            {
                return bpm;
            }
        }

        SortedList<long, List<SongEntry>> notes;
        
        public IEnumerable<IEnumerable<SongEntry>> Notes
        {
            get
            {
                return notes.Values;
            }
        }

        public Song(int TicksPerQuarterNote, double Bpm, IEnumerable<SongEntry> notes)
        {
            this.ticksPerQuarterNote = TicksPerQuarterNote;
            this.bpm = Bpm;
            this.notes = new SortedList<long, List<SongEntry>>();

            foreach (var item in notes)
            {
                List<SongEntry> noteList;
                if (this.notes.TryGetValue(item.EventStart, out noteList))
                {
                    noteList.Add(item);
                }
                else
                {
                    noteList = new List<SongEntry>();
                    noteList.Add(item);
                    this.notes.Add(item.EventStart, noteList);
                }
            }
        }
    }
}
