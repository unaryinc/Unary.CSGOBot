using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Unary.CSGOBot.Abstract;
using System.IO;
using Unary.IOManager.Native;

namespace Unary.CSGOBot.Systems
{
    public class SCFG : ISystem
    {
        private SGameState GameState;
        private Stopwatch Timer;
        private long Timeout;

        private string Path;

        private Queue<string> ChatMessages;
        private List<string> Commands;

        public override void Init()
        {
            GameState = Sys.Ref.Get<SGameState>();
            Timer = new Stopwatch();
            Timeout = Sys.Ref.Get<SConfig>().Get<long>("ChatDelay");

            Path = Directory.GetParent(Sys.Ref.Get<SConfig>().Get<string>("CSGOPath")).FullName + "/csgo/cfg/botexec.cfg";

            ChatMessages = new Queue<string>();
            Commands = new List<string>();
            Timer.Start();
        }

        public override void Clear()
        {
            
        }

        public void MessageAll(string Message)
        {
            ChatMessages.Enqueue("say " + Message);
            Sys.Ref.Console.Message("[SAY] " + Message);
        }

        public void MessageTeam(string Message)
        {
            ChatMessages.Enqueue("say_team " + Message);
            Sys.Ref.Console.Message("[SAY_TEAM] " + Message);
        }

        public void Command(string Command)
        {
            Commands.Add(Command);
        }

        public override void Poll()
        {
            if (Timer.ElapsedMilliseconds >= Timeout && GameState.State == SGameState.GameState.InGame)
            {
                string NewCFG = default;

                if(ChatMessages.Count != 0)
                {
                    NewCFG += ChatMessages.Dequeue() + Environment.NewLine;
                }

                foreach(var Entry in Commands)
                {
                    NewCFG += Entry + Environment.NewLine;
                }

                Commands.Clear();

                File.WriteAllText(Path, NewCFG);

                Sys.Ref.Get<SIO>().IO.Keyboard.KeyPress(VirtualKeyCode.F7);

                Timer.Restart();
            }
        }
    }
}
