using MidReader.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MidReader
{
    class TrackData
    {
        public int Delay { get; set; }
        public float Speed { get; set; }
        public int[] Rhythm { get; set; }
        public List<Event> events { get; set; }
        public class Event
        {
            public int Delay { get; set; }
            public EventTypes Type { get; set; }
            public int Key { get; set; }
            public int Strength { get; set; }
        }
    }
}
