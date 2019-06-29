using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MidReader.Utils;

namespace MidReader
{
    class MidFile
    {
        FileStream s;
        public TrackTypes TrackType { get; set; }
        public List<TrackData> Tracks { get; set; }
        public int MetaTick { get; set; }
        public float TrackSpeed { get; set; }

        private bool _BeginNote = false;
        private int track_count=0;
        public MidFile(FileStream stream)
        {
            s = stream;
            BeginRead();
        }

        private void BeginRead()
        {
            string head_info = Encoding.ASCII.GetString(ReadRAW(4));
            if(head_info== "MThd")
            {
                //4D 54 68 64 00 00 00 06 ff ff nn nn dd dd
                ReadIntRAW();
                TrackType = (TrackTypes)ReadInt16RAW();
                track_count = ReadInt16RAW();
                MetaTick = ReadInt16RAW();
                Tracks = new List<TrackData>();
                for (int i = 0; i < track_count; i++)
                {
                    Tracks.Add(ReadNextTrack());
                }
            }
        }
        private TrackData ReadNextTrack()
        {
            TrackData track;
            string track_info = Encoding.ASCII.GetString(ReadRAW(4));
            if (track_info == "MTrk")
            {
                int length = ReadIntRAW();
                List<byte> data = new List<byte>(ReadRAW(length));
                track = new TrackData();
                track.events = new List<TrackData.Event>();
                while (data.Count>0)
                {
                    TrackData.Event e = new TrackData.Event();
                    e.Delay = readInterval(data);
                    int type = ReadByte(data);
                    if (type == 0xFF)
                    {
                        e.Type = EventTypes.Other;
                        int code = ReadByte(data);
                        int param_len = ReadByte(data);
                        byte[] param_data = ReadData(param_len, data);
                        switch (code)
                        {
                            case 0x51:
                                track.Speed = getIntFromat(param_data);
                                TrackSpeed = track.Speed;
                                break;
                            case 0x58:
                                track.Rhythm = new int[2];
                                track.Rhythm[0] = param_data[0];
                                track.Rhythm[1] = param_data[1];
                                break;
                            case 0x2F:
                                return track;
                        }
                    }else if(type >= 0x90 && type < 0xA0)
                    {
                        e.Type = EventTypes.Note;
                        e.Key = ReadByte(data);
                        e.Strength = ReadByte(data);
                        _BeginNote = true;
                    }
                    else if (type <= 0x7F)
                    {
                        e.Type = EventTypes.Note;
                        e.Key = type;
                        e.Strength = ReadByte(data);
                    }
                    track.events.Add(e);
                }
            }
            return null;
        }
        private int readInterval(List<byte> cache)
        {
            byte d = 0;
            int result = 0;
            List<byte> t = new List<byte>();
            while (true)
            {
                d = ReadData(1, cache)[0];
                if (d <= 0x80)
                {
                    result += d;
                    break;
                }
                else
                {
                    t.Add(d);
                }
            }
            for (int i = 0; i < t.Count; i++)
            {
                result += (t[i] - 128) * (int)Math.Pow(128, t.Count - i);
            }
            return result;
        }
        private int getIntFromat(byte[] param)
        {
            int result = 0;
            Array.Reverse(param);
            for (int i=0;i<param.Length;i++)
            {
                result += param[i] * (int)Math.Pow(256, i);
            }
            return result;
        }
        private byte[] ReadRAW(int length)
        {
            if (length > 0)
            {
                byte[] cache = new byte[length];
                s.Read(cache, 0, length);
                return cache;
            }
            return new byte[] { };
        }
        private byte[] ReadData(int offset, List<byte> cache)
        {
            if (offset > 0)
            {
                byte[] result = cache.Take(offset).ToArray();
                cache.RemoveRange(0, offset);
                return result;
            }
            return new byte[] { };
        }
        private int ReadIntRAW()
        {
            byte[] tmp = ReadRAW(4);
            Array.Reverse(tmp);
            return BitConverter.ToInt32(tmp, 0);
        }
        private int ReadInt16RAW()
        {
            byte[] tmp = ReadRAW(2);
            Array.Reverse(tmp);
            return BitConverter.ToInt16(tmp, 0);
        }
        private int ReadInt(List<byte> cache)
        {
            byte[] tmp = ReadData(4,cache);
            Array.Reverse(tmp);
            return BitConverter.ToInt32(tmp, 0);
        }
        private int ReadInt16(List<byte> cache)
        {
            byte[] tmp = ReadData(2, cache);
            Array.Reverse(tmp);
            return BitConverter.ToInt16(tmp, 0);
        }
        private byte ReadByte(List<byte> cache)
        {
            return ReadData(1, cache)[0];
        }
    }
}
