using System;
using System.Collections.Generic;

namespace Basm.Architecture.X64.Parsing.Intel
{
    internal sealed class InstructionNode : IInstructionNode
    {
        public Instruction Instruction { get; set; }

        public IEnumerable<IExpression> Operands { get; set; }
    }

    internal sealed class BinaryExpression : IBinaryExpression
    {
        public BinaryOperator Operator { get; set; }

        public IExpression Left { get; set; }

        public IExpression Right { get; set; }
    }

    internal sealed class PointerExpression : IPointerExpression
    {
        public PointerType Type { get; set; }

        public IExpression Expression { get; set; }
    }

    internal sealed class RegisterNode : IRegisterNode
    {
        public Register Register { get; set; }
    }
    internal sealed class CastExpression : ICastExpression
    {
        /// <summary>
        /// The type to cast the operand to.
        /// </summary>
        public Type CastType { get; set; }

        /// <summary>
        /// The expression to cast.
        /// </summary>
        public IExpression Operand { get; set; }
    }
    internal sealed class ImmediateValueNode : IImmediateValueNode
    {
        public object Expression { get; set; }
    }
    public enum NodeType
    {
        Identifier,
        Char,
        Byte,
        String,
        BinaryExpression,
        PointerExpresion,
        Comment,
        Statement,
        Integer,
        Section,
        Cast,
        Directive,
        Assignment,
        AccessModifier,
        Value,
        Label,
        InstructionBlock,
        ImmediateValue,
        UnaryExpression,
        Register,
    }
    public class Node : INode
    {
        public Node()
        {

        }
        public Node(string name)
        {
            Value = name;
        }
        internal Node(string name, NodeType type)
        {
            Value = name;
            Type = type;
        }
        public void SetChildCount(int count)
        {
            Children = new INode[count];
        }

        public void SetChild(int index, INode node)
        {
            if (index < Children.Length)
            {
                Children[index] = node;
            }
        }

        public Node NextChild()
        {
            if ((ChildIndex + 1) <= Children.Length)
            {
                return (Node)Children[ChildIndex++];
            }
            return null;
        }
        public Node Child()
        {
            if ((ChildIndex + 1) <= Children.Length)
            {
                return (Node)Children[ChildIndex];
            }
            return null;
        }
        public NodeType GetNodeType()
        {
            return Type;
        }
        public string GetNodeValue()
        {
            return Value;
        }
        public INode[] Children;
        // public INode[] IChildren;
        private int ChildIndex;
        private string Value;
        private NodeType Type;
    }
}
