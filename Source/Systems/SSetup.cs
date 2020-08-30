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

using Unary.CSGOBot.Abstract;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

using Unary.CSGOBot.Utils;

namespace Unary.CSGOBot.Systems
{
    public class SSetup : ISystem
    {
        SLocale Locale;
        SConfig Config;

        public override void Init()
        {
            Locale = Sys.Ref.Get<SLocale>();
            Config = Sys.Ref.Get<SConfig>();

            Sys.Ref.Console.Message(string.Format(Locale.Get("Welcome"), Sys.Ref.Version));

            if (!Sys.Ref.Get<SConfig>().FirstLaunch) { return; }

            Sys.Ref.Console.Message(Locale.Get("FirstTime"));

            Commands();
            Language();
            GamePath();
            SteamID();

            Config.Set("UseVoiceChat", false);
            Config.Set("IODelay", (long)1200);
            Config.Set("PollDelay", 34);

            Sys.Ref.Console.Message(Locale.Get("OtherSettings"));

            Sys.Ref.Console.Message(Locale.Get("Ready"));
        }

        private void Commands()
        {
            Sys.Ref.Console.Message(Locale.Get("CloseCSGO"));
            Sys.Ref.Console.Message(Locale.Get("PressEnter"));
            System.Console.ReadKey();
            Sys.Ref.Console.Message(Locale.Get("AddOption"));
            Sys.Ref.Console.Message(Locale.Get("OptionPath"));
            Sys.Ref.Console.Message(Locale.Get("PressEnter"));
            System.Console.ReadKey();
            Sys.Ref.Console.Message(Locale.Get("LaunchGame"));
            Sys.Ref.Console.Message(Locale.Get("PressEnter"));
            System.Console.ReadKey();
            Sys.Ref.Console.Message(Locale.Get("AddBind"));
            Sys.Ref.Console.Message(Locale.Get("PressEnter"));
            System.Console.ReadKey();
        }

        private void Language()
        {
            while (true)
            {
                Sys.Ref.Console.Message(Locale.Get("SelectLanguage"));
                Sys.Ref.Console.Message(Locale.Get("LanguageWarn"));

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
                            Sys.Ref.Console.Message(string.Format(Locale.Get("SelectedLaungage"), Locale.AvailableLocales[SelectedIndex]));
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

                Sys.Ref.Console.Error(string.Format(Locale.Get("FailedLanguage"), SelectedLocale));
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
                        Sys.Ref.Console.Message(string.Format(Locale.Get("ProvidedCSGOPath"), CSGOPath.FileName));
                        Config.Set("CSGOPath", CSGOPath.FileName);
                        break;
                    }
                }

                Sys.Ref.Console.Error(Locale.Get("InvalidCSGOPath"));
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

                Sys.Ref.Console.Error(string.Format(Locale.Get("InvalidSteamID"), SteamID));
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
