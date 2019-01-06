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
        List<double> freqLookup = null;

        private List<double> getLut()
        {
            if (freqLookup == null)
            {
                var lut = new List<double>();
                lut.Add(16.35);
                lut.Add(17.32);
                lut.Add(18.35);
                lut.Add(19.45);
                lut.Add(20.6);
                lut.Add(21.83);
                lut.Add(23.12);
                lut.Add(24.5);
                lut.Add(25.96);
                lut.Add(27.5);
                lut.Add(29.14);
                lut.Add(30.87);
                lut.Add(32.7);
                lut.Add(34.65);
                lut.Add(36.71);
                lut.Add(38.89);
                lut.Add(41.2);
                lut.Add(43.65);
                lut.Add(46.25);
                lut.Add(49);
                lut.Add(51.91);
                lut.Add(55);
                lut.Add(58.27);
                lut.Add(61.74);
                lut.Add(65.41);
                lut.Add(69.3);
                lut.Add(73.42);
                lut.Add(77.78);
                lut.Add(82.41);
                lut.Add(87.31);
                lut.Add(92.5);
                lut.Add(98);
                lut.Add(103.83);
                lut.Add(110);
                lut.Add(116.54);
                lut.Add(123.47);
                lut.Add(130.81);
                lut.Add(138.59);
                lut.Add(146.83);
                lut.Add(155.56);
                lut.Add(164.81);
                lut.Add(174.61);
                lut.Add(185);
                lut.Add(196);
                lut.Add(207.65);
                lut.Add(220);
                lut.Add(233.08);
                lut.Add(246.94);
                lut.Add(261.63);
                lut.Add(277.18);
                lut.Add(293.66);
                lut.Add(311.13);
                lut.Add(329.63);
                lut.Add(349.23);
                lut.Add(369.99);
                lut.Add(392);
                lut.Add(415.3);
                lut.Add(440);
                lut.Add(466.16);
                lut.Add(493.88);
                lut.Add(523.25);
                lut.Add(554.37);
                lut.Add(587.33);
                lut.Add(622.25);
                lut.Add(659.25);
                lut.Add(698.46);
                lut.Add(739.99);
                lut.Add(783.99);
                lut.Add(830.61);
                lut.Add(880);
                lut.Add(932.33);
                lut.Add(987.77);
                lut.Add(1046.5);
                lut.Add(1108.73);
                lut.Add(1174.66);
                lut.Add(1244.51);
                lut.Add(1318.51);
                lut.Add(1396.91);
                lut.Add(1479.98);
                lut.Add(1567.98);
                lut.Add(1661.22);
                lut.Add(1760);
                lut.Add(1864.66);
                lut.Add(1975.53);
                lut.Add(2093);
                lut.Add(2217.46);
                lut.Add(2349.32);
                lut.Add(2489.02);
                lut.Add(2637.02);
                lut.Add(2793.83);
                lut.Add(2959.96);
                lut.Add(3135.96);
                lut.Add(3322.44);
                lut.Add(3520);
                lut.Add(3729.31);
                lut.Add(3951.07);
                lut.Add(4186.01);
                lut.Add(4434.92);
                lut.Add(4698.63);
                lut.Add(4978.03);
                lut.Add(5274.04);
                lut.Add(5587.65);
                lut.Add(5919.91);
                lut.Add(6271.93);
                lut.Add(6644.88);
                lut.Add(7040);
                lut.Add(7458.62);
                lut.Add(7902.13);

                freqLookup = lut;
            }
            return freqLookup;
        }

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
