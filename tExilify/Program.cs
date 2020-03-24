using System;
using System.IO;
using dnlib;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using MethodBody = dnlib.DotNet.Writer.MethodBody;

namespace tExilify
{
    internal class Program
    {
        /// <summary>
        /// Basic summary of this program
        /// It's main goal is to inject the entry point loader into the Terraria Server,
        /// this can be the tModLoader server.
        /// Which in turn will allow plugins on modded servers :D
        /// To do this we will have one library, which has all the shit we need.
        /// Then we inject it with dnlib.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {

            if (args.Length != 1)
            {
                // We want the file name to be args[0] so check for that.
                Console.WriteLine(@"Incorrect arguments!\nExample: tExilify.exe C:\TerrariaServer.exe");
                return;
            }
            // Now let's go ahead and make sure the file is an actual file y'know, cuz people are stupid.
            if (!File.Exists(args[0]))
            {
                Console.WriteLine(@"That isn't a file!");
                return;
            }
            
            Console.WriteLine("Working...");
            
            // Alright now that we confirmed it's a file, lets dnlib it.
            ModuleDef module = ModuleDefMD.Load(args[0]);
            ModuleDef ModLoader = ModuleDefMD.Load("tExilifyLib.dll");
            
            var ModClass = ModLoader.Types[2];
             
            
            ModLoader.Types.Remove(ModClass); // Remove it because IL is weird
            module.Types.Add(ModClass); // Add it to the original
            
            MethodDef meth2 = findMethod(ModLoader.Types[1], "LoadCode");


            TypeDef type = findType(module.Assembly, "Terraria.Main");
            MethodDef meth = findMethod(type, "startDedInput");

            Console.WriteLine("Loader: " + ModClass.FullName);
            Console.WriteLine("Injecting into " + meth.FullName);
            
            // Add the load call
            
            Console.WriteLine("Almost done...");
            
            CilBody body = new CilBody();
            Console.WriteLine("Writing instructions...");

            foreach (Instruction i in meth2.Body.Instructions)
                if (i.OpCode != OpCodes.Ret)
                    body.Instructions.Add(i);

            foreach (Instruction i in meth.Body.Instructions)
                body.Instructions.Add(i);
            
            body.KeepOldMaxStack = true;
            
            // Replace the old body
            meth.Body = body;
            
            // Write it!
            module.Write("tExiledServer.exe");
            
            Console.WriteLine("Saved!");
            Console.ReadLine();
        }
        
        
        private static MethodDef findMethod(TypeDef type, string methodName)
        {
            if (type != null)
            {
                foreach (var method in type.Methods)
                {
                    if (method.Name == methodName)
                        return method;
                }
            }
            return null;
        }
        
        private static TypeDef findType(AssemblyDef asm, string classPath)
        {
            foreach (var module in asm.Modules)
            {
                foreach (var type in module.Types)
                {
                    if (type.FullName == classPath)
                        return type;
                }
            }
            return null;
        }

    }
}