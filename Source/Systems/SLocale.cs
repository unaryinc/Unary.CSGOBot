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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

using Unary.CSGOBot.Abstract;

namespace Unary.CSGOBot.Systems
{
    public class SLocale : ISystem
    {
        private string Path;
        private SConfig Config;

        public List<string> AvailableLocales { get; private set; }
        private Dictionary<string, string> TargetLocales;
        private Dictionary<string, string> FallbackLocales;

        public override void Init()
        {
            Path = Sys.Ref.Get<SData>().Path + "/Locales";

            AvailableLocales = new List<string>();
            TargetLocales = new Dictionary<string, string>();
            FallbackLocales = new Dictionary<string, string>();

            foreach(var Entry in Directory.GetDirectories(Path))
            {
                AvailableLocales.Add(System.IO.Path.GetFileNameWithoutExtension(Entry));
            }

            SetLocale("English", "English");
        }

        public override void PostInit()
        {
            Config = Sys.Ref.Get<SConfig>();
            SetLocale(Config.Get<string>("TargetLocale"), Config.Get<string>("FallbackLocale"));
        }

        private void Load(string LocaleName, ref Dictionary<string, string> Target)
        {
            foreach(var Entry in Directory.GetFiles(Path + "/" + LocaleName, "*.*"))
            {
                string FileData = File.ReadAllText(Entry);

                try
                {
                    var LocaleData = JsonConvert.DeserializeObject<Dictionary<string, string>>(FileData);

                    foreach (var Locale in LocaleData)
                    {
                        Target[Locale.Key] = Locale.Value;
                    }
                }
                catch(Exception Exception)
                {
                    Sys.Ref.Console.Error("Failed to parse locale at " + Entry + " with exception " + Exception.Message);
                }
            }
        }

        public void SetLocale(string PrimalTarget, string FallbackLocale)
        {
            Load(PrimalTarget, ref TargetLocales);
            Load(FallbackLocale, ref FallbackLocales);
        }

        public string Get(string LocaleKey)
        {
            if(TargetLocales.ContainsKey(LocaleKey))
            {
                return TargetLocales[LocaleKey];
            }
            else if(FallbackLocales.ContainsKey(LocaleKey))
            {
                return FallbackLocales[LocaleKey];
            }
            else
            {
                Sys.Ref.Console.Error("Failed to get any locale for the key " + LocaleKey + ", using the key instead");
                return LocaleKey;
            }
        }

        public override void Clear()
        {
            
        }

        public override void Poll()
        {
            
        }
    }
}
