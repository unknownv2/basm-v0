using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Basm.Assembler.Directives
{
    public sealed class RegisterSymbolDirective : IDirective
    {

        public RegisterSymbolDirective()
        {

        }

        public string Name { get; } = "RegisterSymbol";

        public IConvertible Handle(IScriptContext context, IConvertible[] args)
        {
            var symbolName = args[0].ToString(CultureInfo.InvariantCulture);

            context.Section.Script.Assembler.Symbols[symbolName] = 0;

            return 0;
        }
    }
}
