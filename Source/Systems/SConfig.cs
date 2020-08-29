using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using Unary.CSGOBot.Abstract;
using Unary.CSGOBot.Structs;
using Unary.CSGOBot.Utils;

namespace Unary.CSGOBot.Systems
{
    public class SConfig : ISystem
    {
		public string Path { get; private set; }
		public bool FirstLaunch { get; private set; }

		private Dictionary<string, object> Variables;
		private Dictionary<string, List<Subscriber>> Subscribers;

		public override void Init()
		{
			Path = Sys.Ref.Get<SData>().Path + "/Config.json";

			Variables = new Dictionary<string, object>();
			Subscribers = new Dictionary<string, List<Subscriber>>();

			if(File.Exists(Path))
            {
				FirstLaunch = false;
				Load();
            }
			else
            {
				FirstLaunch = true;
            }
		}

        public override void Clear()
        {
			Save();
        }

        public void Load()
		{
			if (!File.Exists(Path))
			{
				Sys.Ref.Console.Error("Tried to init config at " + Path + " but it is not presented");
				return;
			}

			string Config = File.ReadAllText(Path);

			if (Config == null || Config == "")
			{
				Sys.Ref.Console.Error("Tried to init config at " + Path + " but it was empty");
				return;
			}

			Dictionary<string, JObject> Entries;

			try
			{
				Entries = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(Config);
			}
			catch (Exception Exception)
			{
				Sys.Ref.Console.Error("Failed to load config at " + Path + " with exception " + Exception.Message);
				return;
			}

			SAssembly Assembly = Sys.Ref.Get<SAssembly>();

			foreach (var Entry in Entries)
			{
				string TypeName = Entry.Value["Type"].ToString();

				Type Type = Assembly.GetType(TypeName);

				if (Type == null)
				{
					Sys.Ref.Console.Error(TypeName + " is not a valid type");
					continue;
				}

				try
				{
					object Test = JsonConvert.DeserializeObject(Entry.Value["Value"].ToString(Formatting.None), Type);
					Set(Entry.Key, Test);
				}
				catch (Exception Exception)
				{
					Sys.Ref.Console.Error("Failed at parse of variable " + Entry.Key + " at " + Path);
					Sys.Ref.Console.Error(Exception.Message);
					continue;
				}
			}
		}

		public void Save()
		{
			try
			{
				File.WriteAllText(Path, JsonConvert.SerializeObject(Variables, Formatting.Indented));
			}
			catch (Exception)
			{
				Sys.Ref.Console.Error("Tried saving config at " + Path + " but failed");
			}
		}

		public void Set(string Variable, object Value)
		{
			Variables[Variable] = Value;

			if (Subscribers.ContainsKey(Variable))
			{
				for (int i = Subscribers[Variable].Count - 1; i >= 0; --i)
				{
					var Subscriber = Subscribers[Variable][i];

					if (Subscriber.Type == SubscriberType.Member)
					{
						Subscriber.Target.GetType().GetProperty(Subscriber.MemberName).SetValue(Subscriber.Target, Get(Variable));
					}
					else
					{
						Subscriber.Target.GetType().GetMethod(Subscriber.MemberName).Invoke(Subscriber.Target, new object[] { Get(Variable) });
					}
				}
			}
		}

		public object Get(string Variable)
        {
			if (Variables.ContainsKey(Variable))
			{
				return Variables[Variable];
			}
			else
			{
				Sys.Ref.Console.Error("Tried getting non-existing variable " + Variable);
				return default;
			}
		}

		public T Get<T>(string Variable)
		{
			if (Variables.ContainsKey(Variable))
			{
				return (T)Variables[Variable];
			}
			else
			{
				Sys.Ref.Console.Error("Tried getting non-existing variable " + Variable);
				return default;
			}
		}

		public void Subscribe(object Target, string MemberName, string Variable, SubscriberType Type)
		{
			if (!Variables.ContainsKey(Variable))
			{
				return;
			}

			if (!Subscribers.ContainsKey(Variable))
			{
				Subscribers[Variable] = new List<Subscriber>();
			}

			Subscriber NewSubscriber = new Subscriber
			{
				Target = Target,
				MemberName = MemberName,
				Type = Type
			};

			Subscribers[Variable].Add(NewSubscriber);

			if (Type == SubscriberType.Member)
			{
				Target.GetType().GetProperty(MemberName).SetValue(Target, Get(Variable));
			}
			else
			{
				Target.GetType().GetMethod(MemberName).Invoke(Target, new object[] { Get(Variable) });
			}
		}
	}
}
