using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Basm.Architecture.X64.Parsing
{
    public sealed class IntelTokenizer : IDisposable
    {
        private static readonly Dictionary<char, TokenType> SingleCharacterTokens = new Dictionary<char, TokenType>
        {
            { '[', TokenType.OpenBracket },
            { ']', TokenType.CloseBracket },
            { '(', TokenType.OpenParentheses },
            { ')', TokenType.CloseParentheses },
            { '{', TokenType.OpenBrace },
            { '}', TokenType.CloseBrace },
            { ',', TokenType.Comma },
            { ':', TokenType.Colon },
            { '+', TokenType.Plus },
            { '-', TokenType.Dash },
            { '*', TokenType.Asterisk },
            { '/', TokenType.ForwardSlash },
        };
        private TextReader _io;
  
        private readonly StringBuilder _temp = new StringBuilder(64);

        private char _char;
        private int _lineIndex;
        private int _charIndex;

        private bool _eof;

        public IntelTokenizer(TextReader io)
        {
            _io = io;
        }

        public void Reset(string line)
        {
            if(_io != null)
                _io.Close();

            _io = new StringReader(line);
        }
        private static bool IsBreakCharacter(char c)
        {
            return SingleCharacterTokens.ContainsKey(c)
                || char.IsWhiteSpace(c);
        }
        private static bool IsEndOfStatement(char c)
        {
            return c == '\n' || c == '\0' || c == '\r';
        }
        public bool OnlyHexInString(string test)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(test, @"\A\b[0-9a-fA-F]+\b\Z");
        }

        public bool Eof()
        {
            return _eof;
        }
        internal Token Next()
        {
            _temp.Clear();
            if (!ReadChar(out char c))
            {
                _eof = true;
                return null;
            }

            if (c == '\r')
            {
                if (!ReadChar(out c))
                {
                    return MakeToken(TokenType.Whitespace);
                }

                if (c == '\n')
                {
                    while (PeekChar(out c) && c == '\r')
                    {
                        NextChar();
                        if (PeekChar(out c) && c == '\n')
                            NextChar();
                    }
                    return MakeToken(TokenType.NewLine, "\r\n");
                }

                _temp.Clear();
                _temp.Append('\r');
                while (ReadChar(out c) && char.IsWhiteSpace(c))
                {
                    // TODO: Handle \n
                    _temp.Append(c);
                }
                return MakeToken(TokenType.Whitespace, _temp.ToString());
            }

            // Check if single character token.
            if (SingleCharacterTokens.TryGetValue(c, out TokenType type))
            {
                return MakeToken(type, c.ToString());
            }

            if (char.IsWhiteSpace(c))
            {
                while (PeekChar(out c) && char.IsWhiteSpace(c))
                {
                    NextChar();
                }
                return MakeToken(TokenType.Whitespace, " ");
            }
            _temp.Append(c);
            // Read in arbitrary string.
            while (PeekChar(out c) && !IsBreakCharacter(c))
            {
                _temp.Append(c);
                NextChar();
            }
            string tmp = _temp.ToString();
            
            if (Enum.IsDefined(typeof(Instruction), tmp.ToUpper()))
            {
                return MakeToken(TokenType.Opcode, tmp);
            }
            if (Enum.IsDefined(typeof(Register), tmp.ToUpper()))
            {
                return MakeToken(TokenType.Register, tmp);
            }
            if (Enum.IsDefined(
                typeof(PointerType), tmp.ToUpper()))
            {
                return MakeToken(TokenType.PointerType, tmp);
            }
            return MakeToken(TokenType.Identifier, tmp);
        }

        internal Token Peek()
        {
            return null;
        }
        private bool PeekChar(out char c)
        {
            var n = _io.Peek();
            if (n == -1)
            {

                c = '\0';
                return false;
            }
            else
            {
                c = (char)n;
                //_char = c;
                return true;
            }
        }
        private bool ReadChar(out char c)
        {
            var n = NextChar();
            if (n == -1)
            {

                c = '\0';
                return false;
            }
            else
            {
                c = (char)n;
                //_char = c;
                return true;
            }
        }

        private int NextChar()
        {
            return _io.Read();
        }

        private Token MakeToken(TokenType type, string value = null)
        {
            value = value ?? _char.ToString();

            return new Token
            {
                Type = type,
                Line = _lineIndex,
                Position = _charIndex - value.Length,
                Value = value ?? _char.ToString(),
            };
        }
        
        public void Dispose()
        {
            _io.Close();
        }
    }

    internal class Token
    {
        public TokenType Type;
        public string Value;
        public int Line;
        public int Position;
    }

    internal enum TokenType
    {
        // Single characters
        Identifier,
        OpenBracket,
        CloseBracket,
        OpenParentheses,
        CloseParentheses,
        OpenBrace,
        CloseBrace,
        Comma,
        Colon,
        Plus,
        Dash,
        Or,

        Asterisk,
        ForwardSlash,
        SingleLineComment,
        // expressions
        LessThanOrEqual,
        LessThan,
        GreaterThanOrEqual,
        GreaterThan,
        Equal,
        NotEqualBinary,

        Section,
        String,
        NewLine,
        Whitespace,

        Byte,
        HexString,
        Instruction,
        Opcode,
        Register,
        Statement,
        Code,
        Address,
        Location,
        Comment,
        PointerType,
        PointerExpression,
        Cast,
        None,
  
    }
}