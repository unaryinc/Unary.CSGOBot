using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unary.CSGOBot.Abstract;

namespace Unary.CSGOBot.Systems
{
    public class SKills : ISystem
    {
        private Regex Damage;
        private int LastHash = 0;

        public override void Init()
        {
            Damage = new Regex("^Damage Given to \".*?\" - (.*?) in .*?$");

            Sys.Ref.Get<SLog>().Subscribe("ParsedKills", SLog.LogSubscriberType.Range,
            "^Player: .*? - Damage Given$", "^Player: .*? - Damage Taken$");

            Sys.Ref.Events.Subscribe("ParsedKills", nameof(OnParsedKills), this);
        }

        public void OnParsedKills(List<string> Messages)
        {
            int NewHash = Messages.GetHashCode();

            if (NewHash != LastHash && Messages.Count > 3)
            {
                for (int i = 2; i < Messages.Count - 2; ++i)
                {
                    Match NewMatch = Damage.Match(Messages[i]);
                    int NewDamage = int.Parse(NewMatch.Groups[1].Value);
                    if (NewDamage < 100)
                    {
                        Sys.Ref.Get<SCFG>().MessageAll("-" + NewDamage);
                    }
                }
            }
        }

    }
}
