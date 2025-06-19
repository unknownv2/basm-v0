using System.IO;
using System.Reflection;
using System.Text;
using Basm.Assembler;
using Basm.Assembler.Directives;
using Basm.Assembler.Implementation;
using Basm.CheatEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Basm.Adapters.CheatEngine.Tests
{
    [TestClass]
    public class AdaptersTests
    {
        internal static class ScriptReader
        {
            private static readonly string ScriptDirectory =
                Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location), "Scripts");

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
        [TestMethod]
        public void TestReadMem()
        {
            IMemoryWriter memWriter = null;

            var script = ScriptReader.Read("TestScript");
            var basm = new BasmAssembler(memWriter);
            basm.RegisterDirective(new AllocDirective(null));
            basm.RegisterDirective(new ReadMemDirective(null));
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
