using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Unary.IOManager;
using Unary.CSGOBot.Abstract;

namespace Unary.CSGOBot.Systems
{
    public class SIO : ISystem
    {
        public IOManager.IOManager IO { get; private set; }

        public override void Init()
        {
            IO = new IOManager.IOManager("csgo");
        }

        public override void PostInit()
        {
            IO.Init();
        }

        public override void Clear()
        {
            
        }

        public override void Poll()
        {
            IO.Poll();
        }
    }
}
