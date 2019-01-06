using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroCommon
{
    public class PianoKeyPressEvent
    {
        public PianoKey Key
        {
            get;set;
        }
        public bool Pressed
        {
            get; set;
        }

        public PianoKeyPressEvent(PianoKey Key, bool Pressed)
        {
            this.Key = Key;
            this.Pressed = Pressed;
        }
        
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }

        public static PianoKeyPressEvent Deserialize(string value)
        {
            return JsonConvert.DeserializeObject<PianoKeyPressEvent>(value);
        }
    }
}
