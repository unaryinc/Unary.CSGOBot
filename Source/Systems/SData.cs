using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Unary.CSGOBot.Abstract;

namespace Unary.CSGOBot.Systems
{
    public class SData : ISystem
    {
        public string Path { get; private set; }

        public override void Init()
        {
            Path = "Data";
        }

        public override void Clear()
        {
            
        }

        public override void Poll()
        {
            
        }
    }
}
