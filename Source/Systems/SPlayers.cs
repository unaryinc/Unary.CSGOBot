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
    public class SPlayers : ISystem
    {
        SGameState GameState;
        SServerType ServerType;

        public string OurSteamID { get; private set; }
        public string OurName { get; private set; }

        Dictionary<string, string> SteamIDToNames;
        Dictionary<string, string> NamesToSteamID;
        Regex PlayerInfo;
        Regex Type;

        public override void Init()
        {
            GameState = Sys.Ref.Get<SGameState>();
            ServerType = Sys.Ref.Get<SServerType>();
            OurSteamID = Sys.Ref.Get<SConfig>().Get<string>("SteamID");
            OurName = default;

            SteamIDToNames = new Dictionary<string, string>();
            NamesToSteamID = new Dictionary<string, string>();
            
            PlayerInfo = new Regex("^#.*? \"(.*?)\" (.*?) (.*?)$");
            Type = new Regex("^type    :  (.*?)$");

            Sys.Ref.Get<SLog>().Subscribe("ParsedStatus", SLog.LogSubscriberType.Range,
            "^hostname: (.*?)$", "#end");

            Sys.Ref.Events.Subscribe("ParsedStatus", nameof(OnParsedStatus), this);
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
                    SteamIDToNames[SteamID] = Name;
                    NamesToSteamID[Name] = SteamID;
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
                return default;
            }
        }

        public override void Poll()
        {
            
        }
    }
}
