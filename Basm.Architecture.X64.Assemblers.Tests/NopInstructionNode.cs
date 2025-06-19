using System.Collections.Generic;
using System.Linq;

namespace Basm.Architecture.X64.Assemblers.Tests
{
    public class NopInstructionNode : IInstructionNode
    {
        public Instruction Instruction => Instruction.NOP;

        public IEnumerable<IExpression> Operands => Enumerable.Empty<IExpression>();
    }
}
