using System.Collections.Generic;
using System.Linq;

namespace Basm.Assembler.Implementation
{
    public sealed class BasmAssembler : IAssembler
    {
        private readonly Dictionary<string, IDirective> _directives;
        public IEnumerable<IDirective> Directives { get; }

        internal readonly List<IScript> _scripts;
        public IReadOnlyCollection<IScript> Scripts { get; }

        internal readonly SymbolCollection _symbols;
        public ISymbolCollection Symbols { get; }

        private readonly IMemoryWriter _memory;

        public BasmAssembler(IMemoryWriter memoryWriter)
            : this(16, 8, 32)
        {
            _memory = memoryWriter;
        }

        public BasmAssembler(int directiveInitialCapacity, int scriptInitialCapacity, int symbolInitialCapacity)
        {
            _directives = new Dictionary<string, IDirective>(directiveInitialCapacity);
            Directives = _directives.Values;

            _scripts = new List<IScript>(scriptInitialCapacity);
            Scripts = _scripts.AsReadOnly();

            _symbols = new SymbolCollection(symbolInitialCapacity);
            Symbols = _symbols;
        }

        public void RegisterModule(string moduleName, long baseAddress)
        {
            if (!moduleName.EndsWith(".dll") && !moduleName.EndsWith(".exe"))
                return;

            Symbols[moduleName] = baseAddress;
        }

        public void RegisterDirective(IDirective directive)
        {
            _directives[directive.Name.ToUpperInvariant()] = directive;
        }

        internal IDirective GetDirective(string name)
        {
            return _directives.TryGetValue(name.ToUpperInvariant(), out IDirective directive)
                ? directive : throw new System.Exception($"Unknown directive {name}.");
        }

        public IScript Assemble(IScriptNode scriptNode)
        {
            var script = new Script(this, scriptNode.Children.OfType<ISectionNode>(), _memory);
            lock (_scripts)
            {
                _scripts.Add(script);
            }
            return script;
        }
    }
}
