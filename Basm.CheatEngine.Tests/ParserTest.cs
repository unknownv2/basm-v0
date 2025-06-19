using NUnit.Framework;
using System;

namespace Basm.CheatEngine.Tests
{
    [TestFixture, Order(100)]
    public class ParserTest
    {
        [Test]
        public void ShouldThrowArgumentNullExceptionForAnyConstructorArgument()
        {
            Assert.Throws<ArgumentNullException>(() => new ScriptParser(null, null));
            Assert.Throws<ArgumentNullException>(() => new ScriptParser(Script.Tokenize("Empty"), null));
        }
        [Test]
        public void ShouldParseUnnamedScript()
        {
            new ScriptParser(Script.Tokenize("UnnamedScript"), null).Parse();
            Assert.Throws<ArgumentNullException>(() => new ScriptParser(null, null));
        }
        [Test]
        public void ShouldParseTestScript1()
        {
            var scriptNode = new ScriptParser(Script.Tokenize("TestScript1"), null).Parse();

            Assert.Throws<ArgumentNullException>(() => new ScriptParser(null, null));
        }
    }
}
