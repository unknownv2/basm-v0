using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Basm.Assembler.Directives
{
    public sealed class LabelDirective : IDirective
    {

        public LabelDirective()
        {
          
        }

        public string Name { get; } = "Label";

        public IConvertible Handle(IScriptContext context, IConvertible[] args)
        {
            var symbolName = args[0].ToString(CultureInfo.InvariantCulture);

            context.Section.Script.Symbols[symbolName] = 0;

            return 0;
        }
    }
}
