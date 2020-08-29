using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unary.CSGOBot.Abstract;

namespace Unary.CSGOBot.Systems
{
    public class SServerType : ISystem
    {
        public enum ServerType
        {
            Local,
            Official,
            Unknown
        };

        private Dictionary<string, ServerType> StringToType;

        public ServerType Type { get; private set; }

        public override void Init()
        {
            StringToType = new Dictionary<string, ServerType>
            {
                ["official dedicated"] = ServerType.Official,
                ["listen"] = ServerType.Local
            };
            Type = ServerType.Unknown;
        }

        public override void Clear()
        {
            
        }

        public void SetType(ServerType NewType)
        {
            Type = NewType;
        }

        public void SetType(string NewType)
        {
            if(StringToType.ContainsKey(NewType))
            {
                Type = StringToType[NewType];
            }
            else
            {
                Type = ServerType.Unknown;
            }
        }
    }
}
