using NUnit.Framework;
using System;

namespace Basm.CheatEngine.Tests
{
    [TestFixture, Order(50)]
    public class TokenizerTest
    {
        [Test]
        public void ShouldThrowArgumentNullExceptionInConstructor()
        {
            Assert.Throws<ArgumentNullException>(() => new Tokenizer(null));
        }

        [Test]
        public void ShouldSupportEmptyStrings()
        {
            Assert.IsNull(Script.Tokenize("Empty").Next());
        }
    }
}