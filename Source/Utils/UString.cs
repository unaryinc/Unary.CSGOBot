using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unary.CSGOBot.Utils
{
    public static class UString
    {
        public static bool IsNumber(string Target)
        {
            for(int i = 0; i < Target.Length; ++i)
            {
                if(Target[i] < 48 && Target[i] > 57)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
