using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unary.CSGOBot.Abstract;

namespace Unary.CSGOBot.Systems
{
    public class SPlayers : ISystem
    {
        SGameState GameState;
        SServerType ServerType;

        public string OurSteamID { get; private set; }
        public string OurName { get; private set; }

        Dictionary<string, string> SteamIDToNames;
        Dictionary<string, string> NamesToSteamID;
        Regex PlayerInfo;
        Regex ChatMessage;
        Regex Type;
        private bool FirstInit = true;

        public override void Init()
        {
            GameState = Sys.Ref.Get<SGameState>();
            ServerType = Sys.Ref.Get<SServerType>();
            OurSteamID = Sys.Ref.Get<SConfig>().Get<string>("SteamID");
            OurName = default;
            SteamIDToNames = new Dictionary<string, string>();
            NamesToSteamID = new Dictionary<string, string>();
            PlayerInfo = new Regex("^#.*? \"(.*?)\" (.*?) (.*?)$");
            ChatMessage = new Regex(@"^(.*?)\u200E : (.*?)$");
            Type = new Regex("^type    :  (.*?)$");

            Sys.Ref.Get<SLog>().Subscribe("ParsedStatus", SLog.LogSubscriberType.Range,
            "^hostname: (.*?)$", "#end");

            Sys.Ref.Get<SLog>().Subscribe("ParsedChatMessage", SLog.LogSubscriberType.Single,
            "^(.*?)\u200E : (.*?)$");

            Sys.Ref.Events.Subscribe("ParsedStatus", nameof(OnParsedStatus), this);
            Sys.Ref.Events.Subscribe("ParsedChatMessage", nameof(OnChatMessage), this);
        }

        public void OnParsedStatus(List<string> Messages)
        {
            OurName = default;
            SteamIDToNames.Clear();
            NamesToSteamID.Clear();

            for (int i = 0; i < 4; ++i) { Messages.RemoveAt(0); }

            Match TypeMatch = Type.Match(Messages[0]);
            ServerType.SetType(TypeMatch.Groups[1].Value);

            for (int i = 0; i < 6; ++i) { Messages.RemoveAt(0); }

            for (int i = 0; i < Messages.Count - 1; ++i)
            {
                MatchCollection NewMatches = PlayerInfo.Matches(Messages[i]);
                string Name = NewMatches[0].Groups[1].Value;
                string SteamID = NewMatches[0].Groups[2].Value;

                if(SteamID == OurSteamID)
                {
                    OurName = Name;
                    Sys.Ref.Get<SCFG>().MessageAll("Found our nickname: " + OurName);
                    Sys.Ref.Get<SCFG>().MessageAll("Successfully initialized all of the stuff.");
                }
                else
                {
                    SteamIDToNames[SteamID] = Name;
                    NamesToSteamID[Name] = SteamID;
                }
            }

            //Sys.Ref.Events.Invoke("OnPlayersUpdate");
            ServerType.SetType(SServerType.ServerType.Official);
        }

        public void OnChatMessage(string Message)
        {
            Match NewMatch = ChatMessage.Match(Message);
            string Username = NewMatch.Groups[1].Value;
            string Text = NewMatch.Groups[2].Value;

            if(Text == "/start" && FirstInit)
            {
                FirstInit = false;
                GameState.SetState(SGameState.GameState.InGame);
                Sys.Ref.Console.Message("Got start!");
                Sys.Ref.Get<SCFG>().Command("status");
            }
            else if(Text == "/stop" && Username == OurName)
            {
                Sys.Running = false;
            }
            else if(Text == "/list")
            {
                string Result = default;
                foreach(var Player in NamesToSteamID)
                {
                    Result += Player.Key + ", ";
                }

                Result = Result.Substring(0, Result.Length - 2);
                Sys.Ref.Get<SCFG>().MessageAll(Result);
            }
        }

        public override void PostInit()
        {

        }

        public override void Clear()
        {
            
        }

        public string GetName(string SteamID)
        {
            if(SteamIDToNames.ContainsKey(SteamID))
            {
                return SteamIDToNames[SteamID];
            }
            else
            {
                Sys.Ref.Console.Error("Tried getting non existing user with SteamID " + SteamID);
                return default;
            }
        }

        public string GetSteamID(string Name)
        {
            if (NamesToSteamID.ContainsKey(Name))
            {
                return NamesToSteamID[Name];
            }
            else
            {
                Sys.Ref.Console.Error("Tried getting non existing user with name " + Name);
                return default;
            }
        }

        public override void Poll()
        {
            
        }
    }
}
