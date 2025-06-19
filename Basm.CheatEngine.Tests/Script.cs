using Basm.Assembler;
using System.IO;
using System.Reflection;
using System.Text;

namespace Basm.CheatEngine.Tests
{
    internal static class Script
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
}
