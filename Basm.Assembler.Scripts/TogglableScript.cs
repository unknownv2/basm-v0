using System;
using System.Collections.Generic;
using System.Linq;

namespace Basm.Assembler.Scripts
{
    public class TogglableScript : IScript, IDisposable
    {
        private readonly IScript _script;
        private bool _enabled;

        public TogglableScript(IScript script)
        {
            _script = script;
        }

        public IAssembler Assembler => _script.Assembler;
        public ISymbolCollection Symbols => _script.Symbols;
        public IReadOnlyCollection<ISection> Sections => _script.Sections;

        public void Enable()
        {
            Enabled = true;
        }

        public void Disable()
        {
            Enabled = false;
        }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                {
                    ExecuteSection(value ? "ENABLE" : "DISABLE");
                    _enabled = value;
                }
            }
        }

        private void ExecuteSection(string section)
        {
            _script.Sections.First(s => s.Name == section).Execute();
        }

        public void Dispose()
        {
            Disable();
        }
    }
}
