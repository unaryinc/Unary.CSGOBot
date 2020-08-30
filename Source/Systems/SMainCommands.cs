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

using Unary.CSGOBot.Abstract;
using Unary.CSGOBot.Structs;

namespace Unary.CSGOBot.Systems
{
    public class SMainCommands : ISystem
    {
        private bool FirstInit = true;

        public override void Init()
        {
            Sys.Ref.Get<SCommands>().Add("start");
            Sys.Ref.Get<SCommands>().Subscribe("start", nameof(OnStart), this);
            Sys.Ref.Get<SCommands>().Add("stop");
            Sys.Ref.Get<SCommands>().Subscribe("stop", nameof(OnStop), this);
            Sys.Ref.Get<SLog>().Subscribe("ParseStopBot", SLog.LogSubscriberType.Single, "^" + Sys.Ref.Get<SLocale>().Get("InvalidCommand") + "stop_bot$");
            Sys.Ref.Events.Subscribe("ParseStopBot", nameof(OnStopConsole), this);
        }

        public override void Clear()
        {
            
        }

        public void OnStart(Message NewMessage)
        {
            if(FirstInit)
            {
                FirstInit = false;
                Sys.Ref.Get<SGameState>().SetState(SGameState.GameState.InGame);
                Sys.Ref.Get<SCFG>().Command("status");
            }
        }

        public void OnStopConsole(string Message)
        {
            Sys.Running = false;
        }

        public void OnStop(Message NewMessage)
        {
            if(NewMessage.ByUs)
            {
                Sys.Running = false;
            }
        }
    }
}
