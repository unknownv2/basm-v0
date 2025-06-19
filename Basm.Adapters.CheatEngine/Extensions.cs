using Basm.Architecture.X64.Adapters.Strings;
using Basm.Architecture.X64.Assemblers.Keystone;
using Basm.Architecture.X64.Parsing;
using Basm.Assembler;
using Basm.Assembler.Scripts;
using Basm.CheatEngine;
using System.IO;

namespace Basm.Adapters.CheatEngine
{
    public static class Extensions
    {
        /// <summary>
        /// Assemble a standard Cheat Engine script using Keystone.
        /// </summary>
        /// <param name="assembler">The assembler instance.</param>
        /// <param name="script">The Cheat Engine script with Intel x86-64 assembly.</param>
        /// <returns>A togglable script.</returns>
        public static TogglableScript AssembleIntelCE(this IAssembler assembler, string script)
        {
            // Create the parser for Intel's X64 syntax.
            var intelX64Parser = new IntelParser();

            // Create an X64 assembler that uses Keystone.
            var x64Assembler = new KeystoneAssembler();

            // Combine the two into a single interface that
            // accepts strings and returns assembled instructions.
            var stringAssembler = new StringInstructionAssembler(
                intelX64Parser,
                x64Assembler);

            // Create a Cheat Engine script tokenizer and parser.
            var scriptTokenizer = new Tokenizer(new StringReader(script));
            var scriptParser = new ScriptParser(scriptTokenizer, stringAssembler);

            // Parse the input script into an IScriptNode.
            var scriptNode = scriptParser.Parse();

            return new TogglableScript(assembler.Assemble(scriptNode));
        }
    }
}
