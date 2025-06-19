using System;
using System.Collections.Generic;
using System.Text;

namespace Basm.Assembler.Directives
{
    public sealed class UnRegisterSymbolDirective : IDirective
    {
        public UnRegisterSymbolDirective()
        {

        }

        public string Name { get; } = "UnregisterSymbol";

        public IConvertible Handle(IScriptContext context, IConvertible[] args)
        {
            return null;
        }
    }
}
