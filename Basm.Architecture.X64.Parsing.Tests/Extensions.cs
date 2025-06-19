using NUnit.Framework;
using System.Linq;

namespace Basm.Architecture.X64.Parsing.Tests
{
    public static class Extensions
    {
        public static T Op1<T>(this IInstructionNode node)
        {
            return OpN<T>(node, 0);
        }

        public static T Op2<T>(this IInstructionNode node)
        {
            return OpN<T>(node, 1);
        }

        public static T Op3<T>(this IInstructionNode node)
        {
            return OpN<T>(node, 2);
        }

        public static T Op4<T>(this IInstructionNode node)
        {
            return OpN<T>(node, 3);
        }

        public static T OpN<T>(this IInstructionNode node, int index)
        {
            Assert.Greater(node.Operands.Count(), index);
            var op = node.Operands.ElementAt(index);
            Assert.IsInstanceOf<T>(op);
            return (T)op;
        }
    }
}
