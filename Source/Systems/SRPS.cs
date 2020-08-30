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
using Unary.CSGOBot.Abstract;
using Unary.CSGOBot.Structs;

namespace Unary.CSGOBot.Systems
{
    public class SRPS : ISystem
    {
        private SLocale Locale;
        private SRNG RNG;

        private enum RPSTypes
        {
            Rock,
            Paper,
            Scissors
        }

        private struct RPSEntry
        {
            public List<RPSTypes> LoseTo;
            public List<RPSTypes> Draw;
        }

        private string First = default;
        private RPSTypes FirstPick;
        private string Second = default;
        private RPSTypes SecondPick;

        private Dictionary<RPSTypes, string> TypeToLocale;
        private Dictionary<RPSTypes, RPSEntry> RPSData;
        private List<RPSTypes> ListOfTypes;

        public override void Init()
        {
            RNG = Sys.Ref.Get<SRNG>();
            Locale = Sys.Ref.Get<SLocale>();

            TypeToLocale = new Dictionary<RPSTypes, string>
            {
                [RPSTypes.Rock] = "RPS.Rock",
                [RPSTypes.Paper] = "RPS.Paper",
                [RPSTypes.Scissors] = "RPS.Scissors"
            };

            RPSData = new Dictionary<RPSTypes, RPSEntry>()
            {
                [RPSTypes.Rock] = new RPSEntry()
                {
                    Draw = new List<RPSTypes>(),
                    LoseTo = new List<RPSTypes>()
                    {
                        RPSTypes.Paper
                    }
                },
                [RPSTypes.Paper] = new RPSEntry()
                {
                    Draw = new List<RPSTypes>(),
                    LoseTo = new List<RPSTypes>()
                    {
                        RPSTypes.Scissors
                    }
                },
                [RPSTypes.Scissors] = new RPSEntry()
                {
                    Draw = new List<RPSTypes>(),
                    LoseTo = new List<RPSTypes>()
                    {
                        RPSTypes.Rock
                    }
                }
            };
            ListOfTypes = new List<RPSTypes>();
            foreach (var Type in TypeToLocale) { ListOfTypes.Add(Type.Key); }

            Sys.Ref.Get<SCommands>().Add("rps");
            Sys.Ref.Get<SCommands>().Subscribe("rps", nameof(OnRPS), this);
        }

        public void OnRPS(Message NewMessage)
        {
            if(First == default)
            {
                First = NewMessage.Username;
                FirstPick = ListOfTypes[RNG.Next(0, ListOfTypes.Count - 1)];
                Sys.Ref.Get<SCFG>().MessageAll(string.Format(Locale.Get("RPS.Picked"), First, Locale.Get(TypeToLocale[FirstPick])));
            }
            else if(Second == default && First != NewMessage.Username)
            {
                Second = NewMessage.Username;
                SecondPick = ListOfTypes[RNG.Next(0, ListOfTypes.Count - 1)];
                Sys.Ref.Get<SCFG>().MessageAll(string.Format(Locale.Get("RPS.Picked"), First, Locale.Get(TypeToLocale[SecondPick])));

                if(FirstPick == SecondPick)
                {
                    Sys.Ref.Get<SCFG>().MessageAll(Locale.Get("RPS.Draw"));
                }
                else if(RPSData[FirstPick].LoseTo.Contains(SecondPick))
                {
                    Sys.Ref.Get<SCFG>().MessageAll(string.Format(Locale.Get("RPS.Won"), Second));
                }
                else
                {
                    Sys.Ref.Get<SCFG>().MessageAll(string.Format(Locale.Get("RPS.Won"), First));
                }

                First = default;
                Second = default;
            }
        }
    }
}
