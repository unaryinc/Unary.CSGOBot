using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unary.CSGOBot.Structs
{
    public enum SubscriberType : byte
    {
        Member,
        Method
    }

    public struct Subscriber
    {
        public object Target;
        public string MemberName;
        public SubscriberType Type;
    }
}
