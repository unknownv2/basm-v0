using Basm.Architecture.X64.Parsing.Tests;
using NUnit.Framework;

namespace Basm.Architecture.X64.Parsing.Intel.Tests
{
    [TestFixture]
    public class IntelParsingTest : ParsingTest
    {
        protected override IInstructionParser MakeParser()
        {
            return new IntelParser();
        }
    }
}
