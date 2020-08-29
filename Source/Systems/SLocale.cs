using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
