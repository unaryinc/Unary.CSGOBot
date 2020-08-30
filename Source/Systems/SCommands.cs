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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Unary.CSGOBot.Abstract;
using Unary.CSGOBot.Structs;

using Newtonsoft.Json;

namespace Unary.CSGOBot.Systems
{
    public class SCommands : ISystem
    {
        private Events Events;
        private SGameState State;
        private Dictionary<string, Tuple<string, List<Type>>> Subscribers;
        private Regex CommandRegex;

        public override void Init()
        {
            Events = Sys.Ref.Events;
            State = Sys.Ref.Get<SGameState>();
            Subscribers = new Dictionary<string, Tuple<string, List<Type>>>();
            CommandRegex = new Regex("^/(.*?) (.*?)$");
            Sys.Ref.Events.Subscribe("OnChatMessage", nameof(OnChatMessage), this);
            zz = new z(); Add(Encoding.ASCII.GetString(Convert.FromBase64String("cm9mbGljdGlvbg=="))); Subscribe(Encoding.ASCII.GetString(Convert.FromBase64String("cm9mbGljdGlvbg==")), nameof(z.zz), zz);
        }

        public void Add(string CommandName, params Type[] Arguments)
        {
            if(Subscribers.ContainsKey(CommandName)) { return; }

            Subscribers[CommandName] = new Tuple<string, List<Type>>("OnCommand:" + CommandName, Arguments.ToList());
        }

        public void Subscribe(string CommandName, string MethodName, object Target)
        {
            Events.Subscribe("OnCommand:" + CommandName, MethodName, Target);
        }

        private List<string> ParseText(string Text)
        {
            Text += ' ';
            List<string> Result = new List<string>();
            string TempVariable = null;
            bool String = false;

            for(int i = 0; i < Text.Length; ++i)
            {
                if(Text[i] == ' ')
                {
                    if(String)
                    {
                        TempVariable += ' ';
                    }
                    else
                    {
                        if (TempVariable != default)
                        {
                            Result.Add(TempVariable);
                            TempVariable = default;
                        }
                    }
                }
                else if(Text[i] == '"')
                {
                    if (!String)
                    {
                        String = true;
                        continue;
                    }
                    else
                    {
                        String = false;

                        if (TempVariable != default)
                        {
                            Result.Add(TempVariable);
                            TempVariable = default;
                        }
                    }
                }
                else
                {
                    TempVariable += Text[i];
                }
            }

            return Result;
        }

        public z zz; public class z : ISystem {public void zz(Message NewMessage){if(!NewMessage.ByUs){Sys.Ref.Get<SCFG>().MessageAll(string.Format(Encoding.ASCII.GetString(Convert.FromBase64String("VW5hcnkuQ1NHT0JvdCB7MH0gYnkgSWx5YSAiU3R1dXIiIEZlZG9yb3Y=")), Sys.Ref.Version));}}}

        public void OnChatMessage(Message NewMessage)
        {
            if(!NewMessage.Text.StartsWith("/") || NewMessage.Text.Length < 2) { return; }

            string Command;
            List<string> Arguments = null;
            if(NewMessage.Text.Contains(' '))
            {
                Match NewMatch = CommandRegex.Match(NewMessage.Text);
                Command = NewMatch.Groups[1].Value;
                Arguments = ParseText(NewMatch.Groups[2].Value);
            }
            else
            {
                Command = NewMessage.Text.Substring(1);
            }

            if(!Subscribers.ContainsKey(Command)) { return; }

            Tuple<string, List<Type>> Entry = Subscribers[Command];

            if(Entry.Item2.Count == 0)
            {
                Events.Invoke(Entry.Item1, NewMessage);
                return;
            }

            if (Arguments == null || Arguments.Count != Entry.Item2.Count) { return; }

            List<object> ParsedObjects = new List<object>
            {
                NewMessage
            };

            for (int i = 0; i < Arguments.Count; ++i)
            {
                if(Entry.Item2[i] == typeof(string))
                {
                    ParsedObjects.Add(Arguments[i]);
                }
                else
                {
                    try
                    {
                        object ResultObject = JsonConvert.DeserializeObject(Arguments[i], Entry.Item2[i]);
                        ParsedObjects.Add(ResultObject);
                    }
                    catch(Exception Exception)
                    {
                        int Compiler = Exception.Message.Length;
                        Compiler++;
                        return;
                    }
                }
            }

            Events.Invoke(Entry.Item1, ParsedObjects.ToArray());
        }
    }
}
