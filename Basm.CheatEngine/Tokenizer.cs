using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Basm.CheatEngine
{
    public sealed class Tokenizer : IDisposable
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
            { '"', TokenType.Quote }
        };
        private static readonly Dictionary<string, TokenType>
            StringTokens = new Dictionary<string, TokenType>
        {
            { "//", TokenType.SingleLineComment },
            {"unregistersymbol", TokenType.Directive },
            {"aobscanmodule", TokenType.Directive },
            {"label", TokenType.Directive },
            {"registersymbol", TokenType.Directive },
            {"readmem", TokenType.SpecialDirective },
            {"alloc", TokenType.Directive },
            {"dealloc", TokenType.Directive },
            {"aobscan", TokenType.Directive },
            {"createthread", TokenType.Directive },
            {"define", TokenType.Directive },
            {"fullaccess", TokenType.Directive },
            {"globalalloc", TokenType.Directive },
            {"loadbinary", TokenType.Directive },
            {"loadlibrary", TokenType.Directive },
            {"aobscanregion", TokenType.Directive },
        };

        private readonly TextReader _realIO;
        private TextReader _io;
        private readonly StringBuilder _temp = new StringBuilder(64);

        private char _char;
        private int _lineIndex;
        private int _charIndex;
        private string _line;
        private bool _eof;
        private bool _breakChar;
        public Tokenizer(TextReader io)
        {
            _realIO = io ?? throw new ArgumentNullException(nameof(io));
            NextLine();
        }

        private static bool IsBreakCharacter(char c)
        {
            return SingleCharacterTokens.ContainsKey(c)
                || char.IsWhiteSpace(c);
        }
        private static bool IsEndOfStatement(char c)
        {
            return c == '\n' || c == '\0'  || c == '\r';
        }
        public bool OnlyHexInString(string test)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(test, @"\A\b[0-9a-fA-F]+\b\Z");
        }

        public bool Eof()
        {
            return _eof;
        }

        public bool IsLabel()
        {
            return _breakChar;
        }

        public string GetLine()
        {
            var line = _line;
            NextLine();
            return line;
        }

        private bool _eol;
        public bool Eol()
        {
            return _eol;
        }
         
        internal string NextLine()
        {
            string line = "";
            if ((line = _realIO.ReadLine()) != null)
            {
                while (line.Trim() == String.Empty &&
                       string.IsNullOrEmpty((line = _realIO.ReadLine())))
                {
                    if (line == null)
                    {
                        _eof = true;
                        return null;
                    }
                }
                _line = line.TrimStart();
                // check to determine whether we are at a label or not
                if (_line.Contains(":"))
                {
                    if (_line.Contains("//"))
                    {
                        // check to make sure the colon is outside a comment 
                        if (_line.IndexOf(':') < line.IndexOf("//"))
                        {
                            _breakChar = true;
                        }
                        else
                        {
                            _breakChar = false;
                        }
                    }
                    else
                    {
                        _breakChar = true;
                    }
                }
                else
                {
                    _breakChar = false;
                }
                _io = new StringReader(_line + '\n');
            }
            return line;
        }
        public Token Next()
        {
            _temp.Clear();
            if (!ReadChar(out char c))
            {
                _eof = true;
                return null;
            }

            if (c == '\n')
            {
                return MakeToken(TokenType.NewLine, "\n");
                if (!ReadChar(out c))
                {
                    return MakeToken(TokenType.Whitespace);
                }

                if (c == '\n')
                {
                    while (PeekChar(out c) && c == '\r')
                    {
                        NextChar();
                        if(PeekChar(out c) && c == '\n')
                            NextChar();
                    }
                    //NextLine();
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
                while (PeekChar(out c) && c != '\n' && char.IsWhiteSpace(c))
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
            if (StringTokens.TryGetValue(_temp.ToString(),
                out type))
            {
                return MakeToken(type, _temp.ToString());
            }
            return MakeToken(TokenType.Identifier, _temp.ToString());
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
                c = (char) n;
                //_char = c;
                return true;
            }
        }

        private int NextChar()
        {
            int n = _io.Read();
            if (n == -1)
            {
                _eol = true;
                var line = NextLine();
                if (!string.IsNullOrEmpty(line))
                {
                    _eol = false;
                    n = _io.Read();
                }
            }
            return n;
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

    public class Token
    {
        public TokenType Type;
        public string Value;
        public int Line;
        public int Position;
    }

    public enum TokenType
    {
        None,
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
        SectionBlock,
        String,
        StringBlock,
        NewLine,
        Whitespace,

        Byte,
        HexString,
        Instruction,
        InstructionSet,
        Code,
        Address,
        Location,
        SpecialDirective,
        Directive,
        Comment,
        Quote
    }
}