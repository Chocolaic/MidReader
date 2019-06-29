using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MidReader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("输入mid文件路径:");
            string path = Console.ReadLine();
            FileStream fs = new FileStream(path,FileMode.Open);
            MidFile mid = new MidFile(fs);
            Console.WriteLine(string.Format("音轨类型:{0} 音轨数:{1} Tick/1拍:{2}",mid.TrackType,mid.Tracks.Count,mid.MetaTick));
            foreach(TrackData  track in mid.Tracks)
            {
                Console.WriteLine("#音轨 节拍{0}/{1} 速度:{2} 事件数目:{3}", track.Rhythm[0], track.Rhythm[1], track.Speed / 1000, track.events.Count);
            }
            Console.WriteLine("任意键退出");
            Console.ReadKey();

        }
    }
}
