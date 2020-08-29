using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Unary.CSGOBot.Abstract;

namespace Unary.CSGOBot.Systems
{
    public class Console : ISystem 
    {
        public void Message(string Text)
        {
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine(Text);
        }

        public void Warning(string Text)
        {
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine(Text);
        }

        public void Error(string Text)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine(Text);
        }

        public override void Init()
        {
            Message("Console initialized!");
        }

        public override void Clear()
        {
            Message("Shutting down console");
        }
    }
}
