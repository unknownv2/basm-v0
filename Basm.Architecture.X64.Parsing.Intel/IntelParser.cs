using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Basm.Architecture.X64.Parsing.Intel;

namespace Basm.Architecture.X64.Parsing
{
    public sealed class IntelParser : IInstructionParser
    {
        private Token _token;
        private Stack<Node> _rootNode = new Stack<Node>();
        private Stack<IExpression> _root = new Stack<IExpression>();
        private IntelTokenizer _tokenizer;
 

        public IInstructionNode Parse(string instruction)
        {
            if (instruction == null)
            {
                throw new ArgumentNullException(nameof(instruction));
            }

            using (var tokenizer = new IntelTokenizer(new StringReader(instruction)))
            {
                _tokenizer = tokenizer;
                // start reading
                Next();
                return CreateINode( Instruction());
            }
            return null;
        }

        private Node Instruction()
        {
            if (!OpCode())
                return null;

            if (IsTokenType(TokenType.Whitespace))
            {
                int n = 0;
                Read(TokenType.Whitespace);
                if (!Eof())
                {
                    n++;
                    Expression();
                    while (GetTokenType() != TokenType.None)
                    {
                        if (IsTokenType(TokenType.Whitespace))
                        {
                            WhiteSpace();
                        }
                        else
                        {
                            Expression();
                        }
                        n++;
                    }
                }
                BuildTree("instruction", n + 1);
            }
            return GetRoot();
        }


        private void Expression()
        {
            Term();
        }

        private void Term()
        {
            Factor();
            while (GetTokenType() >= TokenType.Plus
                && GetTokenType() <= TokenType.Or)
            {
                var type = GetTokenType();
                Read(type);
                Factor();
                BuildTree(type, 2);
                //BuildBinExpNode(type, 2);
            }
        }
        private void Factor()
        {
            SpaceString();
            while (IsTokenType(TokenType.Asterisk))
            {
                var type = GetTokenType();
                Read(type);
                SpaceString();
                BuildTree(type, 2);
            }
        }
        private void SpaceString()
        {
            Function();
            if (IsTokenType(TokenType.Whitespace))
            {
                Read(TokenType.Whitespace);
            }
        }
        void Function()
        {
            Primary();
            if (IsTokenType(TokenType.OpenParentheses))
            {
                Read(TokenType.OpenParentheses);
                Expression();
                int n = 1;
                while (IsTokenType(TokenType.Comma))
                {
                    Read(TokenType.Comma);
                    Expression();
                    n++;
                }

                Read(TokenType.CloseParentheses);
                BuildTree("function", n + 1);
            }
        }
        private int Comment()
        {
            if (IsTokenType(TokenType.ForwardSlash))
            {
                Read(TokenType.ForwardSlash);
                Read(TokenType.ForwardSlash);
                int n = 0;
                while (!Eof() && !IsTokenType(TokenType.NewLine))
                {
                    String();
                    n++;
                }
                BuildTree(TokenType.Comment, n);
                return 1;
            }
            if (IsTokenType(TokenType.OpenBrace))
            {
                Read(TokenType.OpenBrace);
                int n = 0;
                while (!IsTokenType(TokenType.CloseBrace))
                {
                    String();
                    n++;
                }
                Read(TokenType.CloseBrace);
                BuildTree(TokenType.Comment, n);
                return 1;
            }
            return 0;
        }
        private void PointerExp()
        {
            //Statement();
            if (IsTokenType(TokenType.PointerType))
            {
                //Read(TokenType.PointerType);
                Name(); // dword or qword
                int n = 1;
                if (IsTokenType(TokenType.Whitespace))
                {
                    Read(TokenType.Whitespace);
                }
                if (IsTokenType(TokenType.Identifier)) //ptr
                {
                    Expression();
                    n++;
                }
                
                Statement();
                BuildTree(TokenType.PointerExpression, n + 1);
            }
        }

        private void Statement()
        {
            Read(TokenType.OpenBracket);
            int n = 1;
            Expression();
            while (!IsTokenType(TokenType.CloseBracket))
            {
                Expression();
                n++;
            }
            Read(TokenType.CloseBracket);
            BuildTree(TokenType.Statement, n);
        }

        private void Cast()
        {
            if (IsTokenType(TokenType.OpenParentheses))
            {
                Read(TokenType.OpenParentheses);
                Name();
                Read(TokenType.CloseParentheses);
                int n = 2;
                if (IsTokenType(TokenType.Dash))
                {
                    Expression();
                    n++;
                }
                Primary();
                BuildTree(TokenType.Cast, n);
            }
        }

        private void Primary()
        {
            switch (GetTokenType())
            {
                case TokenType.PointerType:
                {
                    PointerExp();
                }
                    break;
                case TokenType.OpenBracket:
                    {
                        Statement();
                    }
                    break;
                case TokenType.Plus:
                    {
                        Read(TokenType.Plus);
                        Primary();
                        BuildTree(TokenType.Plus, 2);
                    }
                    break;
                case TokenType.HexString:
                    {
                        int n = 1;
                        HexString();
                        while (IsTokenType(TokenType.HexString))
                        {
                            HexString();
                            n++;
                        }
                        BuildTree(TokenType.HexString, n);
                    }
                    break;
                case TokenType.Whitespace:
                    {
                        WhiteSpace();
                    }
                    break;
                case TokenType.Identifier:
                    {
                        Name();

                    }
                    break;
                case TokenType.Comma:
                case TokenType.Dash:
                    {
                        Name();
                    }
                    break;
                case TokenType.NewLine:
                    break;
                case TokenType.OpenParentheses:
                    {
                        Cast();
                    }
                    break;
                case TokenType.Register:
                {
                    Register();
                }
                    break;
                case TokenType.ForwardSlash:
                {
                    Comment();
                }
                    break;
                default:
                    {
                        throw new Exception("invalid type");
                    }
            }
        }

        private bool OpCode()
        {
            if (IsTokenType(TokenType.Opcode))
            {
                string id = _token.Value;
                Push(id, NodeType.Register);
                Read(id);
                BuildTree(TokenType.Opcode, 1);
                return true;
            }
            return false;
        }

        private void Register()
        {
            if (IsTokenType(TokenType.Register))
            {
                string id = _token.Value;
                Push(id, NodeType.Register);
                Read(id);
                BuildTree(TokenType.Register, 1);
            }
        }
        private void HexString()
        {
            if (IsTokenType(TokenType.Byte))
            {
                Byte();
            }
            else
            {
                Name();
            }
        }

        private void Byte()
        {
            string id = _token.Value;
            Push(id, NodeType.Byte);
            Read(id);
            BuildTree(TokenType.Byte, 1);
        }
        private void WhiteSpace()
        {
            string id = _token.Value;
            Push(id, NodeType.String);
            Read(id);
            BuildTree(TokenType.Whitespace, 1);
        }
        private void Name()
        {
            string id = _token.Value;
            Push(id, NodeType.String);
            Read(id);
            BuildTree(TokenType.Identifier, 1);
        }

        private void String()
        {
            string id = _token.Value;
            Push(id, NodeType.String);
            Read(id);
            BuildTree(TokenType.Identifier, 1);
        }
        private void Next()
        {
            if (!Eof())
            {
                _token = _tokenizer.Next();
            }
        }

        private bool Read(TokenType expected)
        {
            var check = IsTokenType(expected);
            Next();
            SkipWhiteSpace();
            return check;
        }

        private void SkipWhiteSpace()
        {
            while (!Eof() && IsTokenType(TokenType.Whitespace))
            {
                Next();
            }
        }
        private bool Read(string expected)
        {
            var check = CheckString(_token.Value, expected);
            Next();
            return check;
        }
        private bool IsTokenType(TokenType expected)
        {
            if (Eof()) 
                return false;

            return _token.Type == expected;
        }

        internal TokenType GetTokenType()
        {
            if (Eof())
                return TokenType.None;
            return _token.Type;
        }
        private bool CheckString(string a, string b)
        {
            if (a == b || (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)))
            {
                return true;
            }
            return false;
        }

        public bool Eof()
        {
            return _tokenizer.Eof();
        }
        private void Push(string id)
        {
            var node = new Node(id);
            _rootNode.Push(node);
        }
        private void Push(string id, NodeType type)
        {
            var node = new Node(id, type);
            _rootNode.Push(node);
        }
        private void BuildTree(string type, int level)
        {
            Node node = new Node(type);
            node.SetChildCount(level);
            for (int i = level - 1; i >= 0; i--)
            {
                var child = Pop();
                node.SetChild(i, child);
            }
            _rootNode.Push(node);
        }
        private void BuildTree(TokenType type, int level)
        {
            Node node = CreateIExpressionNode(type);
            node.SetChildCount(level);
            for (int i = level - 1; i >= 0; i--)
            {
                var child = Pop();
                node.SetChild(i, child);
            }
            _rootNode.Push(node);
        }
        private Node GetRoot()
        {
            return Pop();
        }

        private Node CreateIExpressionNode(TokenType type)
        {
            Node node = null;
            switch (type)
            {
                case TokenType.Comment:
                    node = new Node(type.ToString(),
                        NodeType.Comment);
                    break;
                case TokenType.Statement:
                case TokenType.PointerExpression:
                    node = new Node(type.ToString(),
                        NodeType.PointerExpresion);
                    break;
                case TokenType.Cast:
                    node = new Node(type.ToString(), NodeType.Cast);
                    break;
                case TokenType.Identifier:
                    node = new Node(type.ToString());
                    break;
                case TokenType.Asterisk:
                case TokenType.Dash:
                case TokenType.ForwardSlash:
                case TokenType.Plus:
                case TokenType.Or:
                    node = new Node(type.ToString(), NodeType.BinaryExpression);
                    break;
                default:
                    node = new Node(type.ToString());
                    break;
            }
            return node;
        }

        private INode CreateINode(NodeType type)
        {
            INode node = null;
            switch (type)
            {
                case NodeType.Value:
                    node = new ImmediateValueNode();
                    break;
                case NodeType.Register:
                    node = new RegisterNode();
                    break;
                case NodeType.Statement:
                    node = new PointerExpression();
                    break;
                case NodeType.BinaryExpression:
                    node = new BinaryExpression();
                    break;
                default:
                    throw new Exception("invalid inode type");
                    node = new Node(type.ToString());
                    break;
            }
            return node;
        }
        private IInstructionNode CreateINode(Node node)
        {
            var inode = new InstructionNode();
            var top = node.NextChild();
            if (Enum.TryParse(((Node)(top.Children[0])).GetNodeValue(), true, out Instruction instruction))
            {
                inode.Instruction = instruction;
                Node curr;
                List<IExpression> expList = new List<IExpression>();
                while ((curr = node.NextChild()) != null)
                {
                    if (!ScreenIExpressionNode(curr))
                        continue;
                    
                    var operand = CreateIExpressionNode(curr);
                    expList.Add(operand);
                }
                inode.Operands = expList;
            }
            return inode;
        }

        /// <summary>
        /// Determine wether a node should be recorded as an operand or not
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool ScreenIExpressionNode(Node node)
        {
            switch (node.GetNodeType())
            {
                case NodeType.Identifier:
                    var data = node.Child().GetNodeValue();
                    if (data == ",")
                        return false;
                    break;
                case NodeType.Comment:
                    return false;
            }
         
            return true;
        }
        private IExpression CreateIExpressionNode(Node cnode)
        {
            IExpression node = null;
            //cnode = cnode.NextChild();

            switch (cnode.GetNodeType())
            {
                case NodeType.Comment:
                    // do nothing for now
                    break;
                case NodeType.Identifier:
                    cnode = cnode.NextChild();
                    node = CreateIExpressionNode(cnode);
                    break;
                case NodeType.Value:
                case NodeType.String:
                    node = CreateImmediateValueNode(cnode);
                    break;
                case NodeType.Register:
                    node = CreateRegNode(cnode);
                    break;
                case NodeType.PointerExpresion:
                case NodeType.Statement:
                    node = CreatePointerExpressionNode(cnode);
                    break;
                case NodeType.Cast:
                    node = CreateCastImmediateValueNode(cnode);
                    break;
                case NodeType.BinaryExpression:
                    node = CreateBinaryExpressionNode(cnode);
                    break;
            }
            return node;
        }
        private IExpression CreateRegNode(Node node)
        {
            if (Enum.TryParse(node.GetNodeValue(), true, out Register reg))
            {
                var regNode = new RegisterNode();
                regNode.Register = reg;
                return regNode;
            }
            return null;
        }
        private IExpression CreateCastImmediateValueNode(Node node)
        {
            var castExp = new CastExpression();
            castExp.CastType = (Type)CreateIExpressionNode(node.NextChild());
            castExp.Operand = CreateIExpressionNode(
                node.NextChild());
            return castExp;
        }
        private IExpression CreateImmediateValueNode(Node node)
        {
            var immNode = new ImmediateValueNode();
            immNode.Expression = node.GetNodeValue();
            return immNode;
        }

        private IExpression CreatePointerExpressionNode(Node node)
        {
            var ptrExp = new PointerExpression();
            ptrExp.Type = PointerType.DWORD;
            Node curr = node;
            while ((curr = node.NextChild()) != null)
            {
                switch (curr.GetNodeType())
                {
                    case NodeType.Identifier:
                        curr = curr.NextChild();
                        if (Enum.TryParse(curr.GetNodeValue(), true,
                                out PointerType pt))
  
                        {
                            ptrExp.Type = pt;
                        }
                        else
                        {
                            ptrExp.Expression = CreateIExpressionNode(curr);
                        }
                        break;
                    case NodeType.BinaryExpression:
                    {
                        var operand = CreateIExpressionNode(curr);
                        ptrExp.Expression = operand;
                    }
                        break;
                    case NodeType.PointerExpresion:
                    case NodeType.Statement:
                        {
                        var operand = CreateIExpressionNode(curr.NextChild());
                            ptrExp.Expression = operand;
                        }
                        break;
                }
            }
        
            return ptrExp;
        }
        private IExpression CreateBinaryExpressionNode(Node node)
        {
            var binExp = new BinaryExpression();
         
            if (Enum.TryParse(node.GetNodeValue(), true, out TokenType tKType))
            {
                binExp.Operator = GetBinaryOperationFromNode(tKType);
            }
            
        
            //var leftExp = Pop();
            //var RightExp = Pop();
            IExpression currentExp = null;
            for (int i = node.Children.Length - 1; i >= 0; i--)
            {
                var child = node.NextChild();
                if (binExp.Left == null)
                {
                    binExp.Left = CreateIExpressionNode(child);
                }
                else
                {
                    binExp.Right = CreateIExpressionNode(child);
                }
            }
            return binExp;
        }
        private BinaryOperator GetBinaryOperationFromNode(TokenType type)
        {
            var binOp = BinaryOperator.Addition;
            switch (type)
            {
                case TokenType.Asterisk:
                    binOp = BinaryOperator.Multiplication;
                    break;
                case TokenType.Dash:
                    binOp = BinaryOperator.Subtraction;
                    break;
                case TokenType.ForwardSlash:
                    binOp = BinaryOperator.Division;
                    break;
            }
            return binOp;
        }
 
        Node Pop()
        {
            Node node = null;
            if (_rootNode.Any())
            {
                node = _rootNode.Pop();
            }
            return node;
        }
    }
}
