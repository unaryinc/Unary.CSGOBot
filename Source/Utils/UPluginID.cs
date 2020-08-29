using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unary.CSGOBot.Utils
{
    public static class UPluginID
    {
        public static string FromType(Type Target)
        {
            if (Target.Namespace != null)
            {
                return Target.FullName.Replace("+", ".");
            }
            else
            {
                return null;
            }
        }
    }
}
