using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Unary.CSGOBot.Abstract;

namespace Unary.CSGOBot.Systems
{
    public class SCommands : ISystem
    {
        private SGameState State;
        private Dictionary<string, string> Subscribers;

        public override void Init()
        {
            State = Sys.Ref.Get<SGameState>();
            Subscribers = new Dictionary<string, string>();
            Sys.Ref.Get<SLog>().Subscribe("OnChatCommand", SLog.LogSubscriberType.Single, "^(.*?) : (.*?)$");
            Sys.Ref.Events.Subscribe("OnChatCommand", nameof(OnChatCommand), this);
        }

        public void OnChatCommand(string Command)
        {
            Sys.Ref.Console.Message("Got command " + Command);
        }
    }
}
