using NUnit.Framework;
using System;

namespace Basm.CheatEngine.Test
{
    [TestFixture]
    public class ValueNodeTest
    {
        [Test]
        public void ShouldParsePositiveInt32()
        {
            Assert.AreEqual(1337, new ValueNode("1337").GetValue(typeof(int)));
        }

        [Test]
        public void ShouldParseNegativeInt32()
        {
            Assert.AreEqual(-1337, new ValueNode("-1337").GetValue(typeof(int)));
        }

        [Test]
        public void ShouldThrowOverflowException()
        {
            Assert.Throws<OverflowException>(() => new ValueNode("12345678912345678").GetValue(typeof(int)));
        }

        [Test]
        public void ShouldParseHexStringsByDefault()
        {
            Assert.AreEqual(0x1337, new ValueNode("1337", true).GetValue(typeof(int)));
        }

        [Test]
        public void ShouldParseLongHexStringsByDefault()
        {
            Assert.AreEqual(0x47C34F80, new ValueNode("47C34F80", true).GetValue(typeof(int)));
        }

        [Test]
        public void ShouldParseHexStringsWhenForced()
        {
            Assert.AreEqual(0x1337, new ValueNode("$1337").GetValue(typeof(int)));
        }

        [Test]
        public void ShouldParseDecimalStringsWhenForced()
        {
            Assert.AreEqual(1337, new ValueNode("#1337", true).GetValue(typeof(int)));
        }

        [Test]
        public void ShouldParseBase10FloatingPointValues()
        {
            Assert.AreEqual(133.7f, new ValueNode("133.7").GetValue(typeof(float)));
        }

        [Test]
        public void ShouldNotParseBase16FloatingPointValues()
        {
            Assert.Throws<ArgumentException>(() => new ValueNode("$4305B333").GetValue(typeof(float)));
        }

        [Test]
        public void ShouldThrowFormatException()
        {
            Assert.Throws<FormatException>(() => new ValueNode("NaN", true).GetValue(typeof(int)));
        }

        [Test]
        public void ShouldParseBools()
        {
            Assert.AreEqual(true, new ValueNode("true").GetValue(typeof(bool)));
            Assert.AreEqual(false, new ValueNode("false").GetValue(typeof(bool)));
            Assert.AreEqual(false, new ValueNode("0").GetValue(typeof(bool)));
            Assert.AreEqual(false, new ValueNode("0.0").GetValue(typeof(bool)));
            Assert.AreEqual(false, new ValueNode("-0.0").GetValue(typeof(bool)));
            Assert.AreEqual(true, new ValueNode("-50").GetValue(typeof(bool)));
            Assert.AreEqual(true, new ValueNode("50").GetValue(typeof(bool)));
        }

        [Test]
        public void ShouldReturnExactInputString()
        {
            var str = "12345";
            var value = new ValueNode(str);
            Assert.AreEqual(str, value.GetValue(typeof(string)));
            Assert.AreEqual(str, value.ToString());
        }

        [Test]
        public void ShouldParseBasicByteArray()
        {
            var array = new ValueNode("01020a0b").GetValue(typeof(byte[])) as byte[];
            Assert.IsNotNull(array);
            Assert.AreEqual(0x01, array[0]);
            Assert.AreEqual(0x02, array[1]);
            Assert.AreEqual(0x0a, array[2]);
            Assert.AreEqual(0x0b, array[3]);
        }

        [Test]
        public void ShouldParseByteArrayWithSpaces()
        {
            var array = new ValueNode("    01  02   0a 0b  ").GetValue(typeof(byte[])) as byte[];
            Assert.IsNotNull(array);
            Assert.AreEqual(0x01, array[0]);
            Assert.AreEqual(0x02, array[1]);
            Assert.AreEqual(0x0a, array[2]);
            Assert.AreEqual(0x0b, array[3]);
        }

        [Test]
        public void ShouldParseWildcardByteArrayWithSpaces()
        {
            var array = new ValueNode("    01  ??   0a **  ").GetValue(typeof(byte?[])) as byte?[];
            Assert.IsNotNull(array);
            Assert.AreEqual(0x01, array[0]);
            Assert.AreEqual(null, array[1]);
            Assert.AreEqual(0x0a, array[2]);
            Assert.AreEqual(null, array[3]);
        }
    }
}
