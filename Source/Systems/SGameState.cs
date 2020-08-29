using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
