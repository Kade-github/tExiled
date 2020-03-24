using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Terraria.tExiled // shits and giggles
{
    public class tExiledLoader
    {
        public static void LoadTExiled()
        {
            // See if the directory exists
            if (!Directory.Exists("tExiled"))
                Directory.CreateDirectory("tExiled");
            // Now lets download all necessary files, or if we have them already we don't need too. (Good for testing test releases for tExiled)
            if (!File.Exists(@"tExiled\tExiled.dll")) // These url's are my website, you can view the files there if you don't trust them :D
                new WebClient().DownloadFile("https://kadedev.software/downloads/tExiled/tExiled.dll", @"tExiled\tExiled.dll");
            if (!File.Exists(@"tExiled\0Harmony.dll"))
                new WebClient().DownloadFile("https://kadedev.software/downloads/tExiled/0Harmony.dll", @"tExiled\0Harmony.dll");
            // Now lets get the raw assembly and load it!
            try
            {
                // Loading code, it's a bit iffy.
                byte[] rawAssembly = File.ReadAllBytes(@"tExiled\tExiled.dll");
                MethodInfo methodInfo = Assembly.Load(rawAssembly).GetTypes()
                    .SelectMany(p => p.GetMethods())
                    .FirstOrDefault(info => info.Name == "TerrariaEntry");
                if (methodInfo != null)
                    methodInfo.Invoke(null,null);
                // Thats it! That's how we loaded tExiled :D
            }
            catch (Exception e)
            {
                throw new FileNotFoundException(@"tExiled Exception - File not found, OR broken download.", "tExiled\tExiled.dll", e);
            }
        }
    }
}