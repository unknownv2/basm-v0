using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using KeystoneNET;

namespace Basm.Architecture.X64.Assemblers.Keystone
{
    public class KeystoneAssembler : IX64InstructionAssembler
    {
        public ulong Address { get; set; }

        public IEnumerable<IUnresolvedExpression> Assemble(IInstructionNode instruction, Stream stream, IExpressionResolver resolver)
        {
            if (instruction == null) throw new ArgumentNullException(nameof(instruction));
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));

            // TODO: Use keystone to assemble the instruction into the stream.

            // Testing tests:
            if (instruction.Instruction == Instruction.NOP)
            {
                //stream.WriteByte(0x90);
                var instStream = Assemble(instruction, resolver);
                stream.Write(instStream, 0, instStream.Length);
                return Enumerable.Empty<IUnresolvedExpression>();
            }
            else
            {
                //stream.WriteByte(0x90);
                var instStream = Assemble(instruction, resolver);
                stream.Write(instStream, 0, instStream.Length);
                return Enumerable.Empty<IUnresolvedExpression>();
            }

            throw new NotImplementedException();
        }

        public string ConvertInstructionToString(IInstructionNode instruction, IExpressionResolver resolver)
        {
            StringBuilder sb= new StringBuilder();
            sb.Append(instruction.Instruction);
            if (instruction.Operands.Any())
            {
                sb.Append(" ");
                int i = 0;
                foreach (var operand in instruction.Operands)
                {
                    sb.Append(resolver.Resolve(operand));
                    if (++i != instruction.Operands.Count())
                        sb.Append(",");
                }
            }
            return sb.ToString();
        }
        public byte[] Assemble(IInstructionNode instruction, IExpressionResolver resolver)
        {
            using (var keystone = new KeystoneNET.Keystone(KeystoneArchitecture.KS_ARCH_X86, KeystoneMode.KS_MODE_64, false))
            {
                var isnStr = ConvertInstructionToString(instruction, resolver);

                var encInstr = keystone.Assemble(isnStr,
                    Address);

                return encInstr.Buffer;
            }

            throw new NotImplementedException();
        }
    }
}
