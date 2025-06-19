using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Basm.Assembler.Directives
{
    public sealed class AobScanModuleDirective : IDirective
    {

        public AobScanModuleDirective()
        {

        }

        public string Name { get; } = "Aobscanmodule";

        public IConvertible Handle(IScriptContext context, IConvertible[] args)
        {
            var symbolName = args[0].ToString(CultureInfo.InvariantCulture);

            context.Section.Script.Symbols[symbolName] = 1;

            return 0;
        }
    }
}
