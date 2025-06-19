using System;
using System.Globalization;

namespace Basm.Assembler.Directives
{
    public sealed class ReadMemDirective : IDirective
    {
        private readonly IMemoryReader _reader;

        public ReadMemDirective(IMemoryReader reader)
        {
            _reader = reader;
        }

        public string Name { get; } = "ReadMem";

        public IConvertible Handle(IScriptContext context, IConvertible[] args)
        {
            var address = args[0].ToInt64(CultureInfo.InvariantCulture);
            var length = args[1].ToInt32(CultureInfo.InvariantCulture);

            if (context.InstructionStream == null)
            {
                throw new InvalidOperationException("ReadMem can only be called within an instruction block.");
            }

            if (_reader == null)
            {
                return null;
                throw new InvalidOperationException("No Memory reader implemented.");
            }

            var instructions = _reader.ReadMemory(address, length);

            context.InstructionStream.Write(instructions, 0, length);

            return null;
        }
    }

    public interface IMemoryReader
    {
        byte[] ReadMemory(long address, int length);
    }
}
