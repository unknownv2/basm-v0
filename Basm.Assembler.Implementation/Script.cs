using System.Collections.Generic;
using System.Linq;

namespace Basm.Assembler.Implementation
{
    internal sealed class Script : IScript
    {
        public Script(BasmAssembler assembler, IEnumerable<ISectionNode> sectionNodes, IMemoryWriter memoryWriter, int initialSymbolCapacity = 16)
        {
            Assembler = assembler;
            Symbols = new SymbolCollection(initialSymbolCapacity);
            Sections = new List<Section>(sectionNodes.Select(node => new Section(assembler, this, node, memoryWriter)));
        }

        public IAssembler Assembler { get; }
        public ISymbolCollection Symbols { get; }
        public IReadOnlyCollection<ISection> Sections { get; }
    }
}
