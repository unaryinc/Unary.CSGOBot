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

using System.Text.RegularExpressions;
using Unary.CSGOBot.Abstract;

namespace Unary.CSGOBot.Systems
{
    public class SChat : ISystem
    {
        private Events Events;
        private SGameState GameState;
        private SPlayers Players;

        private Regex ChatMessage;
        private string DeadString;
        private string CTTeamString;
        private string TTeamString;

        private string DeadCT;
        private string DeadT;
        private string CT;
        private string T;
        private string Dead;

        public override void Init()
        {
            Events = Sys.Ref.Events;
            GameState = Sys.Ref.Get<SGameState>();
            Players = Sys.Ref.Get<SPlayers>();

            ChatMessage = new Regex(@"^(.*?)\u200E : (.*?)$");
            DeadString = Sys.Ref.Get<SLocale>().Get("Dead");
            CTTeamString = Sys.Ref.Get<SLocale>().Get("CTTeam");
            TTeamString = Sys.Ref.Get<SLocale>().Get("TTeam");

            DeadCT = DeadString + CTTeamString + " ";
            DeadT = DeadString + TTeamString + " ";
            CT = CTTeamString + " ";
            T = TTeamString + " ";
            Dead = DeadString + " ";

            Sys.Ref.Get<SLog>().Subscribe("ParsedChatMessage", SLog.LogSubscriberType.Single,
            "^(.*?)\u200E : (.*?)$");
            Sys.Ref.Events.Subscribe("ParsedChatMessage", nameof(OnParsedMessage), this);
        }

        public override void Clear()
        {
            
        }

        private void ParsePrefixes(ref string Result, ref bool IsTeam, ref bool IsDead)
        {
            if(Result.StartsWith(DeadCT))
            {
                IsTeam = true;
                IsDead = true;
                Result = Result.Substring(DeadCT.Length);
            }
            else if(Result.StartsWith(DeadT))
            {
                IsTeam = true;
                IsDead = true;
                Result = Result.Substring(DeadT.Length);
            }
            else if(Result.StartsWith(CT))
            {
                IsTeam = true;
                IsDead = false;
                Result = Result.Substring(CT.Length);
            }
            else if(Result.StartsWith(T))
            {
                IsTeam = true;
                IsDead = false;
                Result = Result.Substring(T.Length);
            }
            else if(Result.StartsWith(Dead))
            {
                IsTeam = false;
                IsDead = true;
                Result = Result.Substring(Dead.Length);
            }
            else
            {
                IsTeam = false;
                IsDead = false;
            }
        }

        public void OnParsedMessage(string Message)
        {
            Match NewMatch = ChatMessage.Match(Message);
            string Username = NewMatch.Groups[1].Value;
            bool IsTeam = false;
            bool IsDead = false;
            ParsePrefixes(ref Username, ref IsTeam, ref IsDead);
            string Text = NewMatch.Groups[2].Value;
            bool WrittenByBot = Text.StartsWith("\u200B");
            if (WrittenByBot) { Text = Text.Substring(1); }
            string SteamID = Players.GetSteamID(Username);
            if(SteamID != null) { SteamID = SteamID.Replace("STEAM_0", "STEAM_1"); }

            Structs.Message NewMessage = new Structs.Message()
            {
                Username = Username,
                SteamID = SteamID,
                ByBot = WrittenByBot,
                ByUs = SteamID == Players.OurSteamID,
                Text = Text,
                TeamChat = IsTeam,
                Dead = IsDead
            };

            if (NewMessage.ByUs && !NewMessage.ByBot)
            {
                Sys.Ref.Get<SCFG>().Prolong = true;
            }

            Events.Invoke("OnChatMessage", NewMessage);
        }
    }
}
