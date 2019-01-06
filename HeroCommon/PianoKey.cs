using System;
using System.Collections.Generic;
using System.Text;

namespace HeroCommon
{
    public enum Note
    {
        A = 1,
        As = 2,
        B = 3,
        C = 4,
        Cs = 5,
        D = 6,
        Ds = 7,
        E = 8,
        F = 9,
        Fs = 10,
        G = 11,
        Gs = 12
    }

    public class PianoKey
    {
        int _octave;
        public int Octave
        {
            get
            {
                return _octave;
            }
        }

        Note _note;
        public Note Note
        {
            get
            {
                return _note;
            }
        }

        public PianoKey(int Octave, Note Note)
        {
            _octave = Octave;
            _note = Note;
        }

        public static PianoKey Parse(string text)
        {
            string noteName = text.Substring(0, text.Length - 1);
            noteName = noteName.Replace('#', 's');

            bool isFlat = false;

            if (noteName.Length > 1 && noteName[1] == 'b')
            {
                isFlat = true;
                var newNoteName = noteName.Substring(0, 1); //+ noteName.Substring(1).Replace("b", "");
                noteName = newNoteName;
            }

            if (Enum.TryParse<Note>(noteName, true, out Note note))
            {
                if (isFlat)
                {
                    note = (note == Note.A) ? Note.Gs : (note - 1);
                }
                int octave = int.Parse(text.Substring(text.Length - 1));
                return new PianoKey(octave, note);
            }
            else
                return null;
        }

        public bool IsSharp
        {
            get
            {
                return (_note == Note.As) ||
                    (_note == Note.Cs) ||
                    (_note == Note.Ds) ||
                    (_note == Note.Fs) ||
                    (_note == Note.Gs);
            }
        }

        public int KeyIndex
        {
            get {
                switch (_note)
                {
                    case Note.A:
                        return 0;
                    case Note.As:
                        return 0;
                    case Note.B:
                        return 1;
                    case Note.C:
                        return 2;
                    case Note.Cs:
                        return 2;
                    case Note.D:
                        return 3;
                    case Note.Ds:
                        return 3;
                    case Note.E:
                        return 4;
                    case Note.F:
                        return 5;
                    case Note.Fs:
                        return 5;
                    case Note.G:
                        return 6;
                    case Note.Gs:
                        return 6;
                }
                return 0;
            }
        }

        public override int GetHashCode()
        {
            int returnVal;
            checked
            {
                returnVal = _octave;
                returnVal += (int)Note;
            }
            return returnVal;
        }

        public override bool Equals(object obj)
        {
            var other = obj as PianoKey;

            bool returnVal = false;
            if (other != null)
            {
                returnVal = (this.Note == other.Note && this.Octave == other.Octave);
            }
            return returnVal;
        }
    }
}
