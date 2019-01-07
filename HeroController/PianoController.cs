using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Linq;
using HeroCommon;

namespace HeroController
{
    class PianoController
    {
        Action<PianoKey, bool> keyChangeAction = null;
        public Action<PianoKey, bool> KeyChangeAction
        {
            get => keyChangeAction;
            set => keyChangeAction = value;
        }


        MidiIn midiIn = null;
        public PianoController()
        {
            if (MidiIn.NumberOfDevices > 0)
            {
                midiIn = new MidiIn(0); 
                midiIn.MessageReceived += MidiIn_MessageReceived;
            }
        }
        
        public void Start()
        {
            if (midiIn != null)
            {
                midiIn.Start();
            }
        }

        private void MidiIn_MessageReceived(object sender, MidiInMessageEventArgs e)
        {
            var ne = e.MidiEvent as NoteEvent;

            if (ne != null)
            {
                var key = PianoKey.Parse(ne.NoteName);

                if (key != null && ne.CommandCode == MidiCommandCode.NoteOn)
                {
                    keyChangeAction?.Invoke(key, true);
                    
                }
                else if (ne.CommandCode == MidiCommandCode.NoteOff)
                {
                    keyChangeAction?.Invoke(key, false);
                }
            }
        }
    }
}
