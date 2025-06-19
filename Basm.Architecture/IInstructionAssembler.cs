using System.Collections.Generic;
using System.IO;

namespace Basm.Architecture
{
    public interface IInstructionAssembler<in TInstruction>
    {
        /// <summary>
        /// Current address of the instruction assembler
        /// </summary>
        ulong Address { get; set; }
        /// <summary>
        /// Assemble an instruction to the given stream.
        /// Unknown immediate values are returned and
        /// expected to be resolved and patched into the
        /// stream at a later time.
        /// </summary>
        /// <param name="instruction">The instruction to assemble.</param>
        /// <param name="stream">The stream to copy the assembled instructions to.</param>
        /// <param name="resolver">A resolver instance used to resolve immediate values.</param>
        /// <returns>Immediate values that couldn't be resolved.</returns>
        IEnumerable<IUnresolvedExpression> Assemble(TInstruction instruction, Stream stream, IExpressionResolver resolver);
    }

    public interface IUnresolvedExpression
    {
        /// <summary>
        /// The offset where the immediate value
        /// is located in the assembled instruction.
        /// </summary>
        long Offset { get; }

        /// <summary>
        /// True if the immediate value must be
        /// converted to an address relative to
        /// instruction it appears in.
        /// </summary>
        bool Relative { get; }

        /// <summary>
        /// The size of the immediate value.
        /// </summary>
        ImmediateValueSize Size { get; }

        /// <summary>
        /// The expression to be resolved into the
        /// immediate value.
        /// </summary>
        object Expression { get; }
    }

    public enum ImmediateValueSize
    {
        B1 = 1,
        B2 = 2,
        B4 = 4,
        B8 = 8,
    }
}
