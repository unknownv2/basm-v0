using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basm.Assembler;

namespace Basm.CheatEngine
{
    public class SectionNode : ISectionNode
    {
        public string Name { get; set;}
        //public IEnumerable<INode> Nodes { get; set; }

        public IEnumerable<INode> Children { get; set; }//=> throw new NotImplementedException();
    }
    public class ScriptNode : IScriptNode
    {
        //public IEnumerable<ISectionNode> Sections { get; set; }

        public IEnumerable<INode> Children { get; set; }// => throw new NotImplementedException();
    }
    public class ImmediateValueNode : ValueNode
    {
        /// <summary>
        /// The expression of the immediate value.
        /// </summary>
        public ImmediateValueNode(string value) : base(
            value, true)
        {
            
        }
        public object Expression { get; set; }
    }
    public class ArgumentNode : IExpression
    {
        /// <summary>
        /// The expression of the immediate value.
        /// </summary>
        public object Expression { get; set; }
    }

    public class LabelNode : ILabelNode
    {
        /// <summary>
        /// Location or name of the memory address.
        /// </summary>
        public IExpression Expression { get; set; }

        /// <summary>
        /// <see cref="IInstructionNode"/>, <see cref="IDirectiveNode"/>, or <see cref="ICommentNode"/> nodes.
        /// </summary>
        public IEnumerable<INode> Children { get; set; }
    }

    public class BinaryExpression : IBinaryExpression
    {
        public BinaryOperator Operator { get; set; }

        public IExpression Left { get; set; }

        public IExpression Right { get; set; }
    }

    public class DirectiveNode : IDirectiveNode
    {
        /// <summary>
        /// The name of the directive.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Directive arguments.
        /// </summary>
        public IEnumerable<IExpression> Arguments { get; set;}
    }

    public class CommentNode : ICommentNode
    {
        public string Comment { get; set;}
    }

    public class Node : INode
    {
        private int ChildIndex;
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
            if (index < Children.Count())
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
        private string Value;
        private NodeType Type;
    }
}
