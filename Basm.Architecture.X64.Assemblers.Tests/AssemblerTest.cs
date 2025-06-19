using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;

namespace Basm.Architecture.X64.Assemblers.Tests
{
    [TestFixture]
    public abstract class AssemblerTest
    {
        protected abstract IX64InstructionAssembler MakeAssembler();

        [Test]
        public void ShouldThrowArgumentNullExceptionForAnyNullArgument()
        {
            var nop = new NopInstructionNode();
            var stream = MakeStream();
            var resolver = new UnresolvingExpressionResolver();

            Assert.Throws<ArgumentNullException>(() => MakeAssembler().Assemble(null, stream, resolver));
            Assert.Throws<ArgumentNullException>(() => MakeAssembler().Assemble(nop, null, resolver));
            Assert.Throws<ArgumentNullException>(() => MakeAssembler().Assemble(nop, stream, null));
        }

        [Test]
        public void ShouldAssembleNopInstruction()
        {
            var unresolved = Assemble(new NopInstructionNode(), out MemoryStream stream);

            Assert.IsEmpty(unresolved);
            Assert.AreEqual(1, stream.Length);
            Assert.AreEqual(0x90, stream.ReadByte());
        }

        protected IEnumerable<IUnresolvedExpression> Assemble(IInstructionNode instruction, out MemoryStream stream, bool resetPosition = true)
        {
            stream = MakeStream();
            return Assemble(instruction, stream);
        }

        protected IEnumerable<IUnresolvedExpression> Assemble(IInstructionNode instruction, MemoryStream stream, bool resetPosition = true)
        {
            var unresolved = MakeAssembler().Assemble(instruction, stream, new UnresolvingExpressionResolver());
            Assert.IsNotNull(unresolved);

            if (resetPosition)
            {
                stream.Position = 0;
            }

            return unresolved;
        }

        protected MemoryStream MakeStream()
        {
            return new MemoryStream(16);
        }
    }
}
