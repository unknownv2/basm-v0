using System;
using System.Globalization;

namespace Basm.Assembler.Directives
{
    public sealed class AllocDirective : IDirective
    {
        private readonly IMemoryAllocator _allocator;

        public AllocDirective(IMemoryAllocator allocator)
        {
            _allocator = allocator;
        }

        public string Name { get; } = "Alloc";

        public IConvertible Handle(IScriptContext context, IConvertible[] args)
        {
            var symbolName = args[0].ToString(CultureInfo.InvariantCulture);
            var size = args[1].ToInt32(CultureInfo.InvariantCulture);
            var allocNear = args.Length > 2 ? args[2].ToInt64(CultureInfo.InvariantCulture) : 0;

            var address = _allocator.AllocateMemory(size, allocNear);

            context.Section.Script.Symbols[symbolName] = address;

            return address;
        }
    }

    public interface IMemoryAllocator
    {
        long AllocateMemory(int size, long near);
        void FreeMemory(long startAddress);
    }
}
