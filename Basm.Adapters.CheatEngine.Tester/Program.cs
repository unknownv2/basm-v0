using System;
using System.IO;
using System.Reflection;
using System.Text;
using Basm.Assembler;
using Basm.Assembler.Directives;
using Basm.Assembler.Implementation;
using Basm.CheatEngine;
using Basm.Memory.Local;
using Basm.Memory.Windows;
using Basm.Memory.Windows.Processes;

namespace Basm.Adapters.CheatEngine.Tester
{
    class Program
    {
        internal static class ScriptReader
        {
            private static readonly string ScriptDirectory = "Scripts";

            private static string GetScriptLocation(string name)
            {
                return Path.Combine(ScriptDirectory, name + ".asm");
            }

            public static string Read(string name)
            {
                return File.ReadAllText(GetScriptLocation(name), Encoding.UTF8);
            }

            public static Tokenizer Tokenize(string name)
            {
                return new Tokenizer(new StringReader(Read(name)));
            }

            public static IScriptNode Parse(string name)
            {
                return new ScriptParser(Tokenize(name), null).Parse();
            }
        }

    
        static void Main(string[] args)
        {
            bool isProcess = false;
            if (isProcess)
            {
                string processName = "BasmTest";
                var procManager = new ProcessManager(processName);
                ProcessMemory pMemory = new ProcessMemory(procManager);
                var script = ScriptReader.Read("TestScript");
                var basm = new BasmAssembler(pMemory);
                basm.RegisterModule($"{processName}.exe", procManager.GetMainModuleBaseAddress());
                basm.RegisterDirective(new AllocDirective(pMemory));
                basm.RegisterDirective(new ReadMemDirective(pMemory));
                basm.RegisterDirective(new LoadLibraryDirective(null));
                basm.RegisterDirective(new LabelDirective());
                basm.RegisterDirective(new AobScanModuleDirective());
                basm.RegisterDirective(new RegisterSymbolDirective());
                basm.RegisterDirective(new UnRegisterSymbolDirective());
                var tgScript = Extensions.AssembleIntelCE(basm, script);
                tgScript.Enable();
            }
            else
            {

                MarshalMemory memWriter = new MarshalMemory();
                var script = ScriptReader.Read("LocalTestScript");
                var basm = new BasmAssembler(memWriter);
                basm.RegisterDirective(new AllocDirective(memWriter));
                basm.RegisterDirective(new ReadMemDirective(memWriter));
                basm.RegisterDirective(new LoadLibraryDirective(null));
                basm.RegisterDirective(new LabelDirective());
                basm.RegisterDirective(new AobScanModuleDirective());
                basm.RegisterDirective(new RegisterSymbolDirective());
                basm.RegisterDirective(new UnRegisterSymbolDirective());
                var tgScript = Extensions.AssembleIntelCE(basm, script);
                tgScript.Enable();
            }
        }
    }
}
