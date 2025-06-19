using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Basm.Assembler.Directives
{
    public class DeAllocDirective : IDirective
    {
        private readonly IMemoryAllocator _allocator;

        public DeAllocDirective(IMemoryAllocator allocator)
        {
            _allocator = allocator;
        }

        public string Name { get; } = "DeAlloc";

        public IConvertible Handle(IScriptContext context, IConvertible[] args)
        {
            // get memory symbol name
            var symbolName = args[0].ToString(CultureInfo.InvariantCulture);
            // free the memory region allocated for the symbol
            if (context.Section.Script.Symbols.ContainsKey(symbolName))
            {
                _allocator.FreeMemory(context.Section.Script.Symbols[symbolName]);
            }
            else if (context.Section.Script.Assembler.Symbols.ContainsKey(symbolName))
            {
                _allocator.FreeMemory(context.Section.Script.Assembler.Symbols[symbolName]);
            }
            return 0;
        }
    }
}
