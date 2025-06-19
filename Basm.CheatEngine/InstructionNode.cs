using Basm.Assembler;
using System.Collections.Generic;
using System.IO;
using Basm.Architecture;
using System;

namespace Basm.CheatEngine
{
    public sealed class InstructionNode : IInstructionNode
    {
        private readonly string _instruction;
        private readonly IExpressionParser _parser;
        private readonly IInstructionAssembler<string> _assembler;
        private Architecture.IExpressionResolver _resolver;
        private long _address;

        public InstructionNode(string instruction, IExpressionParser parser, IInstructionAssembler<string> assembler, Architecture.IExpressionResolver resolver)
        {
            _instruction = instruction;
            _parser = parser;
            _assembler = assembler;
            _resolver = resolver;
        }

        public void SetAddress(ulong instructionAddress)
        {
            _assembler.Address = instructionAddress;
        }
        public void SetResolver(IScriptContext context)
        {
            _resolver = new InstructionResolver(context);
        }
        public IEnumerable<IUnresolvedImmediateValue> Assemble(Stream stream, Assembler.IExpressionResolver resolver)
        {
            foreach (var unresolvedStringExpression in _assembler.Assemble(_instruction, stream, _resolver))
            {
                var expression = unresolvedStringExpression.Expression as string;
                if (expression == null)
                {
                    throw new ArgumentException("Unsupported expression type. Expected a string.");
                }

                // Parse unresolved string expressions to assembler nodes.
                yield return new UnresolvedImmediateValue
                {
                    Offset = unresolvedStringExpression.Offset,
                    Relative = unresolvedStringExpression.Relative,
                    Size = (Assembler.ImmediateValueSize)unresolvedStringExpression.Size,
                    Expression = null,//_parser.Parse(expression),
                };
            }
        }
 
        private sealed class UnresolvedImmediateValue : IUnresolvedImmediateValue
        {
            public long Offset { get; set; }

            public bool Relative { get; set; }

            public Assembler.ImmediateValueSize Size { get; set; }

            public IExpression Expression { get; set; }
        }
    }

    // TODO: Some Cheat Engine class should implement this.
    public interface IExpressionParser
    {
        IExpression Parse(string expression);
    }
}