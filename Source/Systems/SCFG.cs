/*
MIT License

Copyright (c) 2020 Unary Incorporated

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Unary.CSGOBot.Abstract;
using Unary.IOManager.Native;

namespace Unary.CSGOBot.Systems
{
    public class SCFG : ISystem
    {
        private SGameState GameState;
        private Stopwatch Timer;
        private long Timeout;
        public bool Prolong = false;
        private string Path;

        private Queue<string> ChatMessages;
        private List<string> Commands;

        public override void Init()
        {
            GameState = Sys.Ref.Get<SGameState>();
            Timer = new Stopwatch();
            Timeout = Sys.Ref.Get<SConfig>().Get<long>("IODelay");

            Path = Directory.GetParent(Sys.Ref.Get<SConfig>().Get<string>("CSGOPath")).FullName + "/csgo/cfg/botexec.cfg";

            ChatMessages = new Queue<string>();
            Commands = new List<string>();
            // Syncing timers to be in order
            Timer.Start();
        }

        public override void Clear()
        {
            
        }

        public void MessageAll(string Message)
        {
            ChatMessages.Enqueue("say \u200B" + Message);
            Sys.Ref.Console.Message("[SAY] " + Message);
        }

        public void MessageTeam(string Message)
        {
            ChatMessages.Enqueue("say_team \u200B" + Message);
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
                if(Prolong)
                {
                    Prolong = false;
                    Timer.Restart();
                    return;
                }

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

                if(NewCFG != default)
                {
                    File.WriteAllText(Path, NewCFG);
                    Sys.Ref.Get<SIO>().IO.Keyboard.KeyPress(VirtualKeyCode.F7);
                }

                Timer.Restart();
            }
        }
    }
}
