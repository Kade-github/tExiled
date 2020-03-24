namespace HelloWorld
{
    public class Plugin : tPlugin
    {
        public static Plugin Instance;
        
        public static void OnEnabled()
        {
            Instance = new Plugin();
            Instance.Enabled();
        }
        
        public static void OnDisabled()
        {
            Instance.Disabled();
        }

        public void Disabled()
        {
            Log("Goodbye world, i've been disabled!");
        }

        public void Enabled()
        {
            // Metadata
            Name = "HelloWorld";
            Author = "KadeDev";
            Version = "1.0";
            
            Log("Hello World, i've been enabled!");
        }
    }
}