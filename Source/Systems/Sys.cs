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
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Unary.CSGOBot.Abstract;
using Unary.CSGOBot.Utils;

namespace Unary.CSGOBot.Systems
{
    class Sys
    {
        public string Version = "0.0.1";

        public static Sys Ref { get; private set; }
        public static bool Running { get; set; } = true;

        public Console Console { get; private set; }
        public Events Events { get; private set; }

        private List<string> Order;
        private Dictionary<string, ISystem> Systems;

        private int Delay;

        public void Init()
        {
            Ref = this;

            Console = new Console();
            Events = new Events();

            Order = new List<string>();
            Systems = new Dictionary<string, ISystem>();
        }

        public void Clear()
        {
            for (int i = Order.Count - 1; i != 0; --i)
            {
                Systems[Order[i]].Clear();
                Systems.Remove(Order[i]);
                Order.RemoveAt(i);
            }
        }

        private void PostInit()
        {
            for (int i = 0; i < Order.Count; ++i)
            {
                Systems[Order[i]].PostInit();
            }
        }

        public void Add<T>(T NewSystem) where T : ISystem
        {
            string TypeName = UPluginID.FromType(NewSystem.GetType());

            if (Systems.ContainsKey(TypeName))
            {
                Console.Error("Tried to add already registered system" + TypeName);
            }
            else
            {
                Order.Add(TypeName);
                Systems[TypeName] = NewSystem;
                Systems[TypeName].Init();
            }
        }

        public T Get<T>() where T : ISystem
        {
            string TypeName = UPluginID.FromType(typeof(T));

            if (Systems.ContainsKey(TypeName))
            {
                return (T)Systems[TypeName];
            }
            else
            {
                Console.Error("Tried getting non-registered system " + TypeName);
                return default;
            }
        }

        public void Run()
        {
            Add(new SIO());
            Add(new SData());
            Add(new SLocale());
            Add(new SConfig());
            Add(new SSetup());
            Add(new SLog());
            Add(new SGameState());
            Add(new SServerType());
            Add(new SAssembly());
            Add(new SCFG());
            Add(new SPlayers());
            Add(new SKills());
            Add(new SVoiceChat());

            PostInit();

            Delay = Get<SConfig>().Get<int>("PollDelay");
        }

        public bool Poll()
        {
            for (int i = 0; i < Order.Count; ++i)
            {
                Systems[Order[i]].Poll();
            }

            Thread.Sleep(Delay);
            return Running;
        }
    }
}