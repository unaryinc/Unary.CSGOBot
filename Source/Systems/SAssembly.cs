/*
MIT License

Copyright (c) 2020 Unary Incorporated

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using Unary.CSGOBot.Abstract;
using Unary.CSGOBot.Utils;

namespace Unary.CSGOBot.Systems
{
    class SAssembly : ISystem
    {
        private Console Console;

        Dictionary<string, Assembly> Assemblies;
        Dictionary<string, Type> NamedTypes;
        Dictionary<Type, string> ActualTypes;

        public override void Init()
        {
            Console = Sys.Ref.Console;

            Assemblies = new Dictionary<string, Assembly>();

            NamedTypes = new Dictionary<string, Type>();
            ActualTypes = new Dictionary<Type, string>();

            Assembly[] NewAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var Entry in NewAssemblies)
            {
                string CurrentName = Entry.GetName().Name;
                
                if (CurrentName.StartsWith("System") 
                || CurrentName.StartsWith("mscorlib")
                || CurrentName.StartsWith("Unary."))
                {
                    Assemblies[CurrentName] = Entry;
                    AddTypes(Assemblies[CurrentName]);
                }
            }
        }

        public override void PostInit()
        {
            //TODO Load plugins
        }

        public override void Clear()
        {
            Assemblies.Clear();
            NamedTypes.Clear();
            ActualTypes.Clear();
        }

        public Assembly GetAssembly(string PluginID)
        {
            if (Assemblies.ContainsKey(PluginID))
            {
                return Assemblies[PluginID];
            }
            else
            {
                Console.Error("Tried to access not registered assembly " + PluginID);
                return null;
            }
        }

        public string GetType(Type Target)
        {
            if (ActualTypes.ContainsKey(Target))
            {
                return ActualTypes[Target];
            }
            else
            {
                Console.Error("Tried to access not registered type " + UPluginID.FromType(Target));
                return null;
            }
        }

        public Type GetType(string Target)
        {
            if (NamedTypes.ContainsKey(Target))
            {
                return NamedTypes[Target];
            }
            else
            {
                Console.Error("Tried to access not registered type " + Target);
                return null;
            }
        }

        private void AddTypes(Assembly TargetAssembly)
        {
            foreach (var Type in TargetAssembly.GetTypes())
            {
                if (Type.Namespace != null && (Type.IsClass || (Type.IsValueType && !Type.IsEnum)))
                {
                    string Key = UPluginID.FromType(Type);

                    if (!NamedTypes.ContainsKey(Key))
                    {
                        NamedTypes[Key] = Type;
                    }

                    if (!ActualTypes.ContainsKey(Type))
                    {
                        ActualTypes[Type] = Key;
                    }
                }
            }
        }
    }
}
