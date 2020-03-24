using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Terraria;
namespace tExiled
{
    public class Loader
    {
        /// <summary>
        /// The method called when Terraria loads
        /// </summary>

        public static string Version = "1.0.0";
        
        public static List<Type> Plugins = new List<Type>();
        
        public static void TerrariaEntry()
        {
            new Thread(() => { StartServer(); }).Start();
        }

        public static byte[] ReadFile(string path)
        {
            FileStream fileStream = File.Open(path, (FileMode) 3);
            byte[] result;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                fileStream.CopyTo(memoryStream);
                result = memoryStream.ToArray();
            }
            fileStream.Close();
            return result;
        }
        
        public static void StartServer()
        {
            Console.Title = "tExiledServer v" + Version + " - " + Main.worldName;
            // Load 0Harmony for the bois
            Console.WriteLine("[tExiled] Loading 0Harmony...");
            Assembly.Load(ReadFile(Path.Combine("tExiled","0Harmony.dll")));
            // Aight now we need to see about the plugin directory
            Console.WriteLine("[tExiled] Checking directories...");
            if (!Directory.Exists(@"tExiled\Plugins"))
                Directory.CreateDirectory(@"tExiled\Plugins");
            if (!Directory.Exists(@"tExiled\Dependencies"))
                Directory.CreateDirectory(@"tExiled\Dependencies");
            // Load deps
            Console.WriteLine("[tExiled] Loading Dependencies...");
            foreach (string s in Directory.GetFiles(@"tExiled\Dependencies"))
            {
                Assembly a = Assembly.Load(s);
                Console.WriteLine("[Dependencies] Loaded " + a.FullName);
            }
            // Load plugins and call their on enables
            Console.WriteLine("[tExiled] Loading Plugins...");
            foreach (string s in Directory.GetFiles(@"tExiled\Plugins"))
            {
                try
                {
                    Assembly a = Assembly.Load(ReadFile(s));
                    Type ty = a.GetTypes().FirstOrDefault(t =>
                    {
                        bool assign = t.BaseType != null && t.BaseType.Name == "tPlugin";
                        #if DEBUG
                            Console.WriteLine("[DEBUG-Plugins] Checking type " + t.Name + "[BaseType:]" + t.BaseType);
                            Console.WriteLine("[DEBUG-Plugins] Returning, " + assign);
                        #endif
                        return assign;
                    });
                    if (ty != null)
                    {
                        ty.GetMethods().FirstOrDefault(m => m.Name == "OnEnabled")?.Invoke(null, null);
                        Plugins.Add(ty);
                    }
                    else
                        Console.WriteLine("[Plugins] Doesn't seem like '" + s + "' is a plugin!");
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine("[Plugins] Failed to load '" + s + "'. Exception:\n" + e);
                }
            }

            Console.WriteLine("[tExiled] Loading complete! Enjoy the server.\n\n: ");
            
            // Tell plugins they shuttin down when we shuttin down
            AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
            {
                foreach (Type t in Plugins)
                     t.GetMethods().FirstOrDefault(m => m.Name == "OnDisabled")?.Invoke(null, null);
            };
            
            // That's it
        }
        
    }
}