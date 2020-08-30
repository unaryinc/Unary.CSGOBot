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

using System.Collections.Generic;
using System.Text.RegularExpressions;

using Unary.CSGOBot.Abstract;

namespace Unary.CSGOBot.Systems
{
    public class SGameState : ISystem
    {
        public enum GameState
        {
            IntroMovie,
            MainMenu,
            LoadingScreen,
            InGame,
            PauseMenu,
            Chat,
            Closed,
            Unknown,
            PreLaunch
        }

        private SIO IO;

        private Dictionary<string, GameState> StringToState;
        public GameState State { get; private set; }

        private Regex StateRegex;

        public override void Init()
        {
            IO = Sys.Ref.Get<SIO>();

            StringToState = new Dictionary<string, GameState>
            {
                ["CSGO_GAME_UI_STATE_INTROMOVIE"] = GameState.IntroMovie,
                ["CSGO_GAME_UI_STATE_MAINMENU"] = GameState.MainMenu,
                ["CSGO_GAME_UI_STATE_LOADINGSCREEN"] = GameState.LoadingScreen,
                ["CSGO_GAME_UI_STATE_INGAME"] = GameState.InGame,
                ["CSGO_GAME_UI_STATE_PAUSEMENU"] = GameState.PauseMenu
            };
            State = GameState.PreLaunch;

            StateRegex = new Regex("^ChangeGameUIState: (.*?) -> (.*?)$");

            Sys.Ref.Get<SLog>().Subscribe("ParsedState", SLog.LogSubscriberType.Single,
            "^ChangeGameUIState: (.*?) -> (.*?)$");

            Sys.Ref.Events.Subscribe("ParsedState", nameof(ParsedState), this);
        }

        public void ParsedState(string Line)
        {
            string NewState = StateRegex.Match(Line).Groups[2].Value;

            if(StringToState.ContainsKey(NewState))
            {
                State = StringToState[NewState];
            }
            else
            {
                State = GameState.Unknown;
            }
        }

        public void SetState(string NewState)
        {
            if(StringToState.ContainsKey(NewState))
            {
                State = StringToState[NewState];
            }
            else
            {
                State = GameState.Unknown;
            }
        }

        public void SetState(GameState NewState)
        {
            State = NewState;
        }

        public override void Clear()
        {
            
        }

        public override void Poll()
        {
            if(State == GameState.InGame)
            {
                if(IO.IO.Keyboard.IsKeyDown(IOManager.Native.VirtualKeyCode.VK_Y) || IO.IO.Keyboard.IsKeyDown(IOManager.Native.VirtualKeyCode.VK_U))
                {
                    State = GameState.Chat;
                }
            }
            else if(State == GameState.Chat)
            {
                if(IO.IO.Keyboard.IsKeyDown(IOManager.Native.VirtualKeyCode.ESCAPE) || IO.IO.Keyboard.IsKeyDown(IOManager.Native.VirtualKeyCode.RETURN))
                {
                    State = GameState.InGame;
                }
            }
        }
    }
}
