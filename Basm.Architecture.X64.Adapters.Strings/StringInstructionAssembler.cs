using Basm.Architecture.X64.Assemblers;
using Basm.Architecture.X64.Parsing;
using System.Collections.Generic;
using System.IO;

namespace Basm.Architecture.X64.Adapters.Strings
{
    public sealed class StringInstructionAssembler : IInstructionAssembler<string>
    {
        private readonly IInstructionParser _parser;
        private readonly IX64InstructionAssembler _assembler;
        public ulong Address
        {
            get => _assembler.Address;
            set => _assembler.Address = value;
        }
        public StringInstructionAssembler(IInstructionParser parser, IX64InstructionAssembler assembler)
        {
            _parser = parser; // ex: IntelParser
            _assembler = assembler; // ex: KeystoneAssembler
        }

        public IEnumerable<IUnresolvedExpression> Assemble(string instruction, Stream stream, IExpressionResolver resolver)
        {
            var node = _parser.Parse(instruction);

            return _assembler.Assemble(node, stream, resolver);
        }
    }
}
