using System;
using System.Collections.Generic;

using Unary.CSGOBot.Abstract;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

using Unary.CSGOBot.Utils;
using System.Runtime.CompilerServices;

namespace Unary.CSGOBot.Systems
{
    public class SSetup : ISystem
    {
        SLocale Locale;
        SConfig Config;
        SIO IO;

        public override void Init()
        {
            Locale = Sys.Ref.Get<SLocale>();
            Config = Sys.Ref.Get<SConfig>();
            IO = Sys.Ref.Get<SIO>();

            Sys.Ref.Console.Message(string.Format(Locale.Get("Welcome"), Sys.Ref.Version));

            if (!Sys.Ref.Get<SConfig>().FirstLaunch) { return; }

            Sys.Ref.Console.Message(Locale.Get("FirstTime"));

            Language();
            GamePath();
            SteamID();

            Config.Set("UseVoiceChat", false);
            Config.Set("ChatDelay", (long)1200);
            Config.Set("LogDelay", (long)1000);
            Config.Set("PollDelay", 34);

            Sys.Ref.Console.Message(Locale.Get("OtherSettings"));

            Sys.Ref.Console.Message(Locale.Get("Ready"));
        }

        private void Language()
        {
            while (true)
            {
                Sys.Ref.Console.Message(Locale.Get("SelectLanguage"));

                for (int i = 0; i < Locale.AvailableLocales.Count; ++i)
                {
                    Sys.Ref.Console.Message((i + 1) + ". " + Locale.AvailableLocales[i]);
                }

                string SelectedLocale = System.Console.ReadLine();

                if (UString.IsNumber(SelectedLocale))
                {
                    try
                    {
                        int SelectedIndex = int.Parse(SelectedLocale) - 1;
                        if (SelectedIndex >= 0 && SelectedIndex < Locale.AvailableLocales.Count)
                        {
                            Locale.SetLocale(Locale.AvailableLocales[SelectedIndex], "English");
                            Config.Set("TargetLocale", Locale.AvailableLocales[SelectedIndex]);
                            Config.Set("FallbackLocale", "English");
                            break;
                        }
                    }
                    catch (Exception Exception)
                    {
                        string HappyCompiler = Exception.Message;
                        HappyCompiler.GetHashCode();
                    }
                }
                else
                {
                    if (Locale.AvailableLocales.Contains(SelectedLocale))
                    {
                        Locale.SetLocale(SelectedLocale, "English");
                        Config.Set("TargetLocale", SelectedLocale);
                        Config.Set("FallbackLocale", "English");
                        break;
                    }
                }
            }
        }

        private void GamePath()
        {
            string ProvideCSGOPath = Locale.Get("ProvideCSGOPath");

            OpenFileDialog CSGOPath = new OpenFileDialog
            {
                InitialDirectory = "C:\\",
                FileName = "csgo.exe",
                Filter = "CSGO EXE|*.exe",
                FilterIndex = 2,
                Title = ProvideCSGOPath
            };

            while (true)
            {
                Sys.Ref.Console.Message(ProvideCSGOPath);
                if (CSGOPath.ShowDialog() == DialogResult.OK)
                {
                    if (Path.GetFileNameWithoutExtension(Directory.GetParent(CSGOPath.FileName).FullName) == "Counter-Strike Global Offensive")
                    {
                        Config.Set("CSGOPath", CSGOPath.FileName);
                        break;
                    }
                }
            }
        }

        private void SteamID()
        {
            while (true)
            {
                Sys.Ref.Console.Message(Locale.Get("ProvideSteamID"));
                Sys.Ref.Console.Message(Locale.Get("SteamIDExample"));
                Sys.Ref.Console.Message(Locale.Get("SteamIDSite"));

                System.Console.ReadLine();

                string SteamID = Clipboard.GetText();

                Sys.Ref.Console.Message("Trying to use " + SteamID + " as a SteamID");

                Regex SteamRegex = new Regex("STEAM_[0-9]:[0-9]:[0-9]*");
                MatchCollection Matches = SteamRegex.Matches(SteamID);

                if(Matches.Count == 1)
                {
                    Config.Set("SteamID", SteamID.Replace("STEAM_0", "STEAM_1"));
                    break;
                }
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
