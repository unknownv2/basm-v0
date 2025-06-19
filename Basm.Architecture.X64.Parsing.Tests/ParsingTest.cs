using NUnit.Framework;
using System;
using System.Linq;

namespace Basm.Architecture.X64.Parsing.Tests
{
    [TestFixture]
    public abstract class ParsingTest
    {
        protected abstract IInstructionParser MakeParser();

        [Test]
        public void ShouldThrowArgumentNullExceptionWithNullInstruction()
        {
            Assert.Throws<ArgumentNullException>(() => MakeParser().Parse(null));
        }

        [Test]
        public void ShouldParseZeroOperandInstruction()
        {
            Parse("nop", Instruction.NOP);
        }

        [Test]
        public void ShouldParseSingleOperandInstructionWithRegister()
        {
            var node = Parse<IRegisterNode>("push eax", Instruction.PUSH);
            Assert.AreEqual(Register.EAX, node.Op1<IRegisterNode>().Register);
        }
        [Test]
        public void ShouldParseSingleOperandInstructionWithRegisterAndImmediateValue()
        {
            var node = Parse<IBinaryExpression>("push eax + 4", Instruction.PUSH);
            var op = node.Op1<IBinaryExpression>();
    
            Assert.AreEqual(BinaryOperator.Addition, op.Operator);
            // Check left side of operator.
            Assert.IsInstanceOf<IRegisterNode>(op.Left);
            Assert.AreEqual(Register.EAX, ((IRegisterNode)op.Left).Register);

            // Check right side of operator.
            Assert.IsInstanceOf<IImmediateValueNode>(op.Right);
            Assert.AreEqual("4", ((IImmediateValueNode)op.Right).Expression);
        }
        [Test]
        public void ShouldParseSingleOperandInstructionWithImmediateValue()
        {
            var node = Parse<IImmediateValueNode>("push 1", Instruction.PUSH);
            Assert.AreEqual("1", node.Op1<IImmediateValueNode>().Expression);
        }

        [Test]
        public void ShouldParseSingleOperandInstructionWithDWordPtrOfRegister()
        {
            var node = Parse<IPointerExpression>("push dword ptr [eax]", Instruction.PUSH);
            var op = node.Op1<IPointerExpression>();
            Assert.AreEqual(PointerType.DWORD, op.Type);
            Assert.IsInstanceOf<IRegisterNode>(op.Expression);
            Assert.AreEqual(Register.EAX, ((IRegisterNode)op.Expression).Register);
        }

        [Test]
        public void ShouldParseSingleOperandInstructionWithPtrOfRegister()
        {
            var node = Parse<IPointerExpression>("push [eax]", Instruction.PUSH);
            var op = node.Op1<IPointerExpression>();
            Assert.AreEqual(PointerType.DWORD, op.Type);
            Assert.IsInstanceOf<IRegisterNode>(op.Expression);
            Assert.AreEqual(Register.EAX, ((IRegisterNode)op.Expression).Register);
        }


        [Test]
        public void ShouldParseSingleOperandInstructionWithDWordPtrOfImmediateValue()
        {
            var node = Parse<IPointerExpression>("push dword ptr [apples]", Instruction.PUSH);
            var op = node.Op1<IPointerExpression>();
            Assert.AreEqual(PointerType.DWORD, op.Type);
            Assert.IsInstanceOf<IImmediateValueNode>(op.Expression);
            Assert.AreEqual("apples", ((IImmediateValueNode)op.Expression).Expression);
        }

        [Test]
        public void ShouldParseSingleOperandInstructionWithDWordPtrOfComplexExpression()
        {
            var node = Parse<IPointerExpression>("push dword ptr [apples + 10]", Instruction.PUSH);

            // Check first operand types.
            var op = node.Op1<IPointerExpression>();
            Assert.AreEqual(PointerType.DWORD, op.Type);
            Assert.IsInstanceOf<IBinaryExpression>(op.Expression);

            // Check inner expression.
            var expr = (IBinaryExpression)op.Expression;
            Assert.AreEqual(BinaryOperator.Addition, expr.Operator);

            // Check left side of operator.
            Assert.IsInstanceOf<IImmediateValueNode>(expr.Left);
            Assert.AreEqual("apples", ((IImmediateValueNode)expr.Left).Expression);

            // Check right side of operator.
            Assert.IsInstanceOf<IImmediateValueNode>(expr.Right);
            Assert.AreEqual("10", ((IImmediateValueNode)expr.Right).Expression);
        }

        [Test]
        public void ShouldParseExpressionWithCastAndComment()
        {
            var node = Parse<IPointerExpression, ICastExpression>
                ("mov [edx+234], (float)999999 //fuel", Instruction.MOV);

            // we ignore the comment
            // Check first operand types.
            var op = node.Op1<IPointerExpression>();
            Assert.AreEqual(PointerType.DWORD, op.Type);
            Assert.IsInstanceOf<IBinaryExpression>(op.Expression);

            // Check inner expression.
            var expr = (IBinaryExpression)op.Expression;
            Assert.AreEqual(BinaryOperator.Addition, expr.Operator);

            // Check left side of operator.
            Assert.IsInstanceOf<IRegisterNode>(expr.Left);
            Assert.AreEqual(Register.EDX, ((IRegisterNode)expr.Left).Register);

            // Check right side of operator.
            Assert.IsInstanceOf<IImmediateValueNode>(expr.Right);
            Assert.AreEqual("234", ((IImmediateValueNode)expr.Right).Expression);

            var op2 = node.Op2<ICastExpression>();
            Assert.IsInstanceOf<IImmediateValueNode>(op2.CastType);
            Assert.IsInstanceOf<IImmediateValueNode>(op2.Operand);

            Assert.AreEqual("float", ((IImmediateValueNode)op2.CastType).Expression);
            Assert.AreEqual("999999", ((IImmediateValueNode)op2.Operand).Expression);
        }
        private IInstructionNode Parse(string instruction, Instruction type, int operands = 0)
        {
            var node = MakeParser().Parse(instruction);
            Assert.IsNotNull(node);
            Assert.AreEqual(type, node.Instruction);
            Assert.AreEqual(operands, node.Operands.Count());
            return node;
        }

        private IInstructionNode Parse<TOp1>(string instruction, Instruction type)
        {
            var node = Parse(instruction, type, 1);
            Assert.IsInstanceOf<TOp1>(node.Operands.ElementAt(0));
            return node;
        }

        private IInstructionNode Parse<TOp1, TOp2>(string instruction, Instruction type)
        {
            var node = Parse(instruction, type, 2);
            Assert.IsInstanceOf<TOp1>(node.Operands.ElementAt(0));
            Assert.IsInstanceOf<TOp2>(node.Operands.ElementAt(1));
            return node;
        }

        private IInstructionNode Parse<TOp1, TOp2, TOp3>(string instruction, Instruction type)
        {
            var node = Parse(instruction, type, 3);
            Assert.IsInstanceOf<TOp1>(node.Operands.ElementAt(0));
            Assert.IsInstanceOf<TOp2>(node.Operands.ElementAt(1));
            Assert.IsInstanceOf<TOp3>(node.Operands.ElementAt(2));
            return node;
        }

        private IInstructionNode Parse<TOp1, TOp2, TOp3, TOp4>(string instruction, Instruction type)
        {
            var node = Parse(instruction, type, 4);
            Assert.IsInstanceOf<TOp1>(node.Operands.ElementAt(0));
            Assert.IsInstanceOf<TOp2>(node.Operands.ElementAt(1));
            Assert.IsInstanceOf<TOp3>(node.Operands.ElementAt(2));
            Assert.IsInstanceOf<TOp4>(node.Operands.ElementAt(3));
            return node;
        }
    }
}
