using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Unary.CSGOBot.Abstract;
using Unary.CSGOBot.Structs;

namespace Unary.CSGOBot.Systems
{
    public class SLog : ISystem
    {
        public enum LogSubscriberType
        {
            Single,
            Range
        }

        private Stopwatch Timer;
        private long Timeout;

        private string Path;
        private bool ReadingRange = false;
        private long FileSize;

        private Dictionary<Regex, string> SingleSubs;

        private List<Regex> RangeBegins;
        private Dictionary<Regex, string> RangeEnds;

        public override void Init()
        {
            Timer = new Stopwatch();

            SingleSubs = new Dictionary<Regex, string>();

            RangeBegins = new List<Regex>();
            RangeEnds = new Dictionary<Regex, string>();

            Timer.Start();

        }

        public override void PostInit()
        {
            Timeout = Sys.Ref.Get<SConfig>().Get<long>("LogDelay");
            Path = Directory.GetParent(Sys.Ref.Get<SConfig>().Get<string>("CSGOPath")).FullName + "/csgo/console.log";
            FileSize = new FileInfo(Path).Length;
        }

        public void Subscribe(string EventName, LogSubscriberType Type, string BeginRegex, string EndRegex = null)
        {
            Regex NewBeginRegex = new Regex(BeginRegex);

            if (Type == LogSubscriberType.Single)
            {
                SingleSubs[NewBeginRegex] = EventName;
            }
            else
            {
                RangeBegins.Add(NewBeginRegex);
                Regex NewEndRegex = new Regex(EndRegex);
                RangeEnds[NewEndRegex] = EventName;
            }
        }

        public override void Clear()
        {

        }

        public override void Poll()
        {
            if(Timer.ElapsedMilliseconds >= Timeout)
            {
                long NewFileSize = new FileInfo(Path).Length;
                long SizeDelta = NewFileSize - FileSize;
                
                if(SizeDelta == 0) { return; }

                string[] Lines;

                using (var FS = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var SR = new StreamReader(FS, Encoding.Default))
                {
                    byte[] Buffer = new byte[SizeDelta];
                    FS.Position = FileSize;
                    FS.Read(Buffer, 0, (int)SizeDelta);

                    Lines = Encoding.UTF8.GetString(Buffer).Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                }

                FileSize = NewFileSize;

                List<string> RangeResult = new List<string>();

                foreach(var Line in Lines)
                {
                    if(ReadingRange)
                    {
                        RangeResult.Add(Line);

                        foreach (var Regex in RangeEnds)
                        {
                            Match NewMatch = Regex.Key.Match(Line);
                            if (NewMatch.Success)
                            {
                                ReadingRange = false;
                                Sys.Ref.Events.Invoke(Regex.Value, RangeResult);
                            }
                        }
                    }
                    else
                    {
                        foreach (var Regex in RangeBegins)
                        {
                            Match NewMatch = Regex.Match(Line);
                            if (NewMatch.Success)
                            {
                                ReadingRange = true;
                                RangeResult.Add(Line);
                            }
                        }
                    }

                    foreach (var Regex in SingleSubs)
                    {
                        Match NewMatch = Regex.Key.Match(Line);
                        if (NewMatch.Success)
                        {
                            Sys.Ref.Events.Invoke(Regex.Value, Line);
                        }
                    }
                }
                Timer.Restart();
            }
        }
    }
}