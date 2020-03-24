using System;

public class tPlugin
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Author { get; set; }

        public void Log(string log)
        {
            Console.WriteLine("[" + Name + "] " + log + "\n\n: ");
        }
    }