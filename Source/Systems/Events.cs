using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unary.CSGOBot.Systems
{
    public class Events
    {
        private Dictionary<string, List<Tuple<string, object>>> Subscribers;

        public Events()
        {
            Subscribers = new Dictionary<string, List<Tuple<string, object>>>();
        }

        public void Invoke(string Event, params object[] Args)
        {
            if(Subscribers.ContainsKey(Event))
            {
                foreach(var Subscriber in Subscribers[Event])
                {
                    var Method = Subscriber.Item2.GetType().GetMethod(Subscriber.Item1);
                    Method.Invoke(Subscriber.Item2, Args);
                }
            }
            else
            {
                Sys.Ref.Console.Error("Tried to invoke an event with no subscribers");
            }
        }

        public void Subscribe(string EventName, string MethodName, object Target)
        {
            if(!Subscribers.ContainsKey(EventName))
            {
                Subscribers[EventName] = new List<Tuple<string, object>>();
            }

            Subscribers[EventName].Add(new Tuple<string, object>(MethodName, Target));
        }
    }
}
