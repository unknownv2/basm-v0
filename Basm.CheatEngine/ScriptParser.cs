using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basm.Assembler;
using Basm.Architecture;

namespace Basm.CheatEngine
{
    public sealed class ScriptParser
    {
        private readonly Tokenizer _tokenizer;
        private readonly IInstructionAssembler<string> _instructionAssembler;
        private readonly InstructionResolver _resolver;
        private Token _token;
        private Stack<Node> _rootNode = new Stack<Node>();
        private bool _isNewLabel;



        public ScriptParser(Tokenizer tokenizer, IInstructionAssembler<string> instructionAssembler)
        {
            _tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
            _instructionAssembler = instructionAssembler ?? throw new ArgumentNullException(nameof(instructionAssembler));
           
        }

        public IScriptNode Parse()
        {
            // start reading
            Next();
            return Start(); 
        }
        public IExpression Parse(string expression)
        {
            // start reading
            Next();
            return null;
        }
        public IScriptNode Start()
        {
            Sections();
            return CreateScriptNode(Pop());
        }

        private ScriptNode CreateScriptNode(Node root)
        {
            var scNode = new ScriptNode();
            Node curr;
            List<SectionNode> sections = new List<SectionNode>();
            while ((curr = root.NextChild()) != null)
            {
                if (curr.GetNodeType() == NodeType.Section)
                {
                    sections.Add(CreateSectionNode(curr));
                } 
            }
            scNode.Children = sections;
            return scNode;
        }

        private SectionNode CreateSectionNode(Node node)
        {
            SectionNode sc = new SectionNode();
            Node curr = node.NextChild();
            sc.Name = curr.NextChild().GetNodeValue();
            node = node.NextChild();
            var nodes = new List<INode>();
            INode inode = null;
            if (node.GetNodeType() == NodeType.SectionBlock)
            {
                while ((curr = node.NextChild()) != null)
                {
                    inode = ReadParentNode(curr);
                    if(inode != null)
                        nodes.Add(inode);
                }
            }
            sc.Children = nodes;
            return sc;
        }

        private INode ReadParentNode(Node node)
        {
            INode inode = null;
            switch (node.GetNodeType())
            {
                case NodeType.Directive:
                    inode = CreateDirectiveNode(node);
                    break;
                case NodeType.Label:
                    inode = CreateLabelNode(node);
                    break;
                case NodeType.Instruction:
                    inode = CreateInstructionNode(node);
                    break;
                case NodeType.Comment:
                    inode = CreateCommentNode(node);
                    break;

            }
            return inode;
        }

        private IEnumerable<INode> CreateInstructionBlock(Node node)
        {
            Node curr = null;
            var children = new List<INode>();
            while ((curr = node.NextChild()) != null)
            {
                children.Add(ReadParentNode(curr));
            }

            return children;
        }

        private InstructionNode CreateInstructionNode(Node node)
        {
            var isn = new InstructionNode(node.NextChild().GetNodeValue(),
                null, _instructionAssembler, null);
            return isn;
        }
        private LabelNode CreateLabelNode(Node node)
        {
            var labelNode = new LabelNode();
            Node curr = node.NextChild();
            labelNode.Expression = CreateIExpressionNode(curr);
            var children = new List<INode>();
            while ((curr = node.NextChild()) != null)
            {
                if (curr.GetNodeType() == NodeType.InstructionBlock)
                {
                    children.AddRange(CreateInstructionBlock(curr));
                }
                else
                {
                    children.Add(ReadParentNode(curr));
                }
            }
            labelNode.Children = children;

            return labelNode;
        }

        private DirectiveNode CreateDirectiveNode(Node node)
        {
            var dn = new DirectiveNode();
            Node curr = node.NextChild();
            dn.Name = curr.NextChild().GetNodeValue();
            var args = new List<IExpression>();
            while((curr = node.NextChild()) != null)
            {
               args.Add(CreateIExpressionNode(curr));
            }
            dn.Arguments = args;
            return dn;
        }
     
        private IExpression CreateIExpressionNode(Node cnode)
        {
            IExpression node = null;

            switch (cnode.GetNodeType())
            {
                case NodeType.Value:
                    node = CreateArgumentNode(cnode);
                    break;
                case NodeType.BinaryExpression:
                    node = CreateBinaryExpressionNode(cnode);
                    break;
                case NodeType.Identifier:
                case NodeType.String:
                case NodeType.StringBlock:
                    node = CreateStringNode(cnode);
                    break;
            }
            return node;
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
        private IExpression CreateBinaryExpressionNode(Node node)
        {
            var binExp = new BinaryExpression();
            if (Enum.TryParse(node.GetNodeValue(), true, out TokenType tKType))
            {
                binExp.Operator = GetBinaryOperationFromNode(tKType);
            };
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

        private CommentNode CreateCommentNode(Node node)
        {
            var commentNode = new CommentNode();
            commentNode.Comment = GetStringFromNode(node);
            return commentNode;
        }

        private IExpression CreateStringNode(Node node)
        {
            var immNode = new ImmediateValueNode(GetStringFromNode(node));
            return immNode;
        }

        private IExpression CreateArgumentNode(Node node)
        {
            return new ImmediateValueNode(GetStringFromNode(node));
        }

        private string GetStringFromNode(Node node)
        {
            Node curr ;
            StringBuilder sb = new StringBuilder();
            while ((curr = node.NextChild()) != null)
            {
                if (curr.GetNodeType() == NodeType.Identifier)
                {
                    curr = curr.NextChild();
                }
                sb.Append(curr.GetNodeValue());
            }
            return sb.ToString();
        }
        private void Sections()
        {
            if (IsTokenType(TokenType.OpenBracket))
            {
                int n = 0;

                while (IsTokenType(TokenType.OpenBracket))
                {
                    Section();
                    n++;
                }
                BuildTree("script", n);
            }
        }

        private void Section()
        {
            Read(TokenType.OpenBracket);
            if (IsTokenType((TokenType.Identifier)))
            {
                Name();
            }
            Read(TokenType.CloseBracket);
            Assembly();
            BuildTree(TokenType.Section, 2);
        }

        private void Assembly()
        {
            if (IsTokenType(TokenType.NewLine))
            {
                int n = 0;
                int k = 0;
                while (!IsTokenType(TokenType.OpenBracket))
                {
                    while (IsTokenType(TokenType.NewLine))
                    {
                        Read(TokenType.NewLine);
                        k += Comment();
                    }
                    if (Eof() && Eol())
                        break;
                   
                    Asm();
                    k += Comment();
                    n++;
    
                    if(Eof() && Eol())
                        break;
                }

                BuildTree(TokenType.SectionBlock, k + n);
            }
        }
        private void Asm()
        {
            int n = 0;
            if (IsTokenType(TokenType.Directive))
            {
                Name();
                int k = Directive();
                if (k != 0)
                {
                    BuildTree(TokenType.Directive, n + k + 1);
                }
            }
            else
            {
                Expression();
                Label();
            }
        }
        private int Directive()
        {
            if (IsTokenType(TokenType.OpenParentheses))
            {
                Read(TokenType.OpenParentheses);
                int n = 1;

                Expression();
                while (IsTokenType(TokenType.Comma))
                {
                    Read(TokenType.Comma);
                    Expression();
                    n++;
                }
                Read(TokenType.CloseParentheses);
                //BuildTree(type, n);
                return n;
            }
            return 0;
        }
        private void Label()
        {
            if (IsTokenType(TokenType.Colon))
            {
                Read(TokenType.Colon);
          
                _isNewLabel = true;
             
                int j = Comment();
                if (IsTokenType(TokenType.NewLine))
                {
                    int n = 0, k = 1;
                    Read(TokenType.NewLine);
                    while (!IsLabel() &&
                        !IsTokenType(TokenType.OpenBracket) &&
                       !IsTokenType(TokenType.Directive)
                        && !Eof())
                    {
                        if (IsTokenType(TokenType.NewLine))
                            Read(TokenType.NewLine);

                        if(IsLabel() || IsTokenType(TokenType.Directive))
                            break;

                        ReadLabelLine();
                        n++;
                    }
                    if (n != 0)
                    {
                        k++;
                        BuildTree(TokenType.InstructionSet, n);
                    }
                    BuildTree(TokenType.Location, k + j);
                }
                _isNewLabel = false;
            }
        }

        private int Comment()
        {
            if(IsTokenType(TokenType.ForwardSlash))
            {
                Read(TokenType.ForwardSlash);
                Read(TokenType.ForwardSlash);
                int n = 0;
                while (!IsTokenType(TokenType.NewLine))
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

        private void InlineDirective()
        {
            if (IsTokenType(TokenType.SpecialDirective))
            {
                Name();
                int k = Directive();
                if (k != 0)
                {
                    k += Comment();
                    BuildTree(TokenType.Directive, k + 1);
                }
            }
        }
        private string GetLine()
        {
            var line = _tokenizer.GetLine();
            Next();
            return line;
        }
  
        private void Expression()
        {
            Term();
            if (IsTokenType(TokenType.Identifier))
            {
                int n = 1;
                SkipWhiteSpace();
                while (!IsTokenType(TokenType.CloseParentheses))
                {
                    Primary();

                    SkipWhiteSpace();

                    n++;
                }
                BuildTree(TokenType.StringBlock, n);
            }
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
            Primary();
            if (IsTokenType(TokenType.Whitespace))
            {
                SkipWhiteSpace();
            }
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
                BuildTree("cast", n);
            }
        }
        private void Quote()
        {
            Read(TokenType.Quote);
            int n = 1;
            Expression();
            while (!IsTokenType(TokenType.Quote))
            {
                Expression();
                n++;
            }
            Read(TokenType.Quote);
            BuildTree("quote", n);
        }
        private void Primary()
        {
            switch (GetTokenType())
            {
                case TokenType.Quote:
                {
                    Quote();
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
                case TokenType.ForwardSlash:
                {
                    Comment();
                }
                break;
                case TokenType.None:
                break;
                default:
                {
                    throw new Exception("invalid type");
                }
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

        private void ReadLabelLine()
        {
            var type = GetTokenType();
            switch (type)
            {
                case TokenType.SpecialDirective:
                    InlineDirective();
                    break;
                case TokenType.ForwardSlash:
                    Comment();
                    break;
                case TokenType.Identifier:
                    Line();
                    break;
            }
        }
        private void Line()
        {
            Push(GetLine(), NodeType.String);
            BuildTree(TokenType.Instruction, 1);
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

        public bool IsLabel()
        {
            return _tokenizer.IsLabel();
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

        private bool Eol()
        {
            return _tokenizer.Eol();
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
            for(int i = level - 1 ; i >=0; i--)
            {
                var child = Pop();
                node.SetChild(i, child);
            }
            _rootNode.Push(node);
        }
        private Node CreateIExpressionNode(TokenType type)
        {
            Node node;
            switch (type)
            {
                case TokenType.Section:
                    node = new Node(type.ToString(),
                        NodeType.Section);
                    break;
                case TokenType.StringBlock:
                    node = new Node(type.ToString(),
                        NodeType.StringBlock);
                    break;
                case TokenType.SectionBlock:
                    node = new Node(type.ToString(),
                        NodeType.SectionBlock);
                    break;
                case TokenType.Comment:
                    node = new Node(type.ToString(),
                        NodeType.Comment);
                    break;
                case TokenType.Instruction:
                    node = new Node(type.ToString(), NodeType.Instruction);
                    break;
                case TokenType.InstructionSet:
                    node = new Node(type.ToString(),
                        NodeType.InstructionBlock);
                    break;
                case TokenType.Location:
                    node = new Node(type.ToString(),
                        NodeType.Label);
                    break;
                case TokenType.Directive:
                case TokenType.SpecialDirective:
                    node = new Node(type.ToString(),
                        NodeType.Directive);
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
 
 
        Node Pop()
        {
            Node node = null;
            if (_rootNode != null && _rootNode.Any())
            {
                node = _rootNode.Pop();
            }
            return node;
        }
    }
}
