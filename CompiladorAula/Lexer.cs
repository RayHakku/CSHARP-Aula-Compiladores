using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiladorAula
{
    public class Lexer
    {
        private readonly string _source;
        private readonly List<Token> _tokens = new List<Token>();

        // Current position in the source code
        private int _start = 0;
        private int _current = 0;
        private int _line = 1;
        private int _column = 1;

        // Dictionary of reserved keywords
        private readonly Dictionary<string, TokenTypes> _keywords;

        public Lexer(string source)
        {
            _source = source;

            // Initialize the dictionary of keywords
            _keywords = new Dictionary<string, TokenTypes>
            {
                { "var", TokenTypes.VAR },
                { "if", TokenTypes.IF },
                { "else", TokenTypes.ELSE },
                { "while", TokenTypes.WHILE },
                { "print", TokenTypes.PRINT },
                { "int", TokenTypes.INT },
                { "string", TokenTypes.STR }
            };
        }


        // Scan all tokens in the source code
        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                // Begin scanning at the current position
                _start = _current;
                ScanToken();
            }

            // Add an EOF token
            _tokens.Add(new Token(TokenTypes.EOF, "", null, _line, _column));
            return _tokens;
        }

        // Scan a single token
        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                // Handle single-character tokens
                case '(': AddToken(TokenTypes.LEFT_PAREN); break;
                case ')': AddToken(TokenTypes.RIGHT_PAREN); break;
                case '{': AddToken(TokenTypes.LEFT_BRACE); break;
                case '}': AddToken(TokenTypes.RIGHT_BRACE); break;
                case ',': AddToken(TokenTypes.COMMA); break;
                case ';': AddToken(TokenTypes.SEMICOLON); break;
                case '+': AddToken(TokenTypes.PLUS); break;
                case '-': AddToken(TokenTypes.MINUS); break;
                case '*': AddToken(TokenTypes.MULTIPLY); break;
                case '/': AddToken(TokenTypes.DIVIDE); break;

                // Handle potentially two-character tokens
                case '=':
                    if (Match('='))
                        AddToken(TokenTypes.EQUAL);
                    else
                        AddToken(TokenTypes.ASSIGN);
                    break;
                case '!':
                    if (Match('='))
                        AddToken(TokenTypes.NOT_EQUAL);
                    else
                        AddToken(TokenTypes.UNKNOWN); // We could handle '!' as NOT in the future
                    break;
                case '<':
                    if (Match('='))
                        AddToken(TokenTypes.LESS_EQUAL);
                    else
                        AddToken(TokenTypes.LESS);
                    break;
                case '>':
                    if (Match('='))
                        AddToken(TokenTypes.GREATER_EQUAL);
                    else
                        AddToken(TokenTypes.GREATER);
                    break;

                // Ignore whitespace
                case ' ':
                case '\r':
                case '\t':
                    // Update column position
                    _column++;
                    break;
                case '\n':
                    _line++;
                    _column = 1; // Reset column on new line
                    break;

                // Handle string literals
                case '"': ScanString(); break;

                default:
                    // Handle numeric literals
                    if (IsDigit(c))
                    {
                        ScanNumber();
                    }
                    // Handle identifiers and keywords
                    else if (IsAlpha(c))
                    {
                        ScanIdentifier();
                    }
                    else
                    {
                        // Unknown token
                        AddToken(TokenTypes.UNKNOWN);
                    }
                    break;
            }
        }

        // Scan a string literal
        private void ScanString()
        {
            StringBuilder value = new StringBuilder();

            // Continue until closing quote or end of file
            while (!IsAtEnd() && Peek() != '"')
            {
                char c = Advance();
                // Handle escaped quotes
                if (c == '\\' && Peek() == '"')
                {
                    value.Append(Advance());
                }
                else
                {
                    value.Append(c);
                }
            }

            // Unterminated string
            if (IsAtEnd())
            {
                // Report error: Unterminated string
                return;
            }

            // Consume the closing "
            Advance();

            // Add the string token
            AddToken(TokenTypes.STRING, value.ToString());
        }

        // Scan a numeric literal
        private void ScanNumber()
        {
            while (IsDigit(Peek()))
            {
                Advance();
            }

            // Get the numeric literal as a string
            string number = _source.Substring(_start, _current - _start);

            // Convert to integer and add token
            AddToken(TokenTypes.INTEGER, int.Parse(number));
        }

        // Scan an identifier or keyword
        private void ScanIdentifier()
        {
            while (IsAlphaNumeric(Peek()))
            {
                Advance();
            }

            // Get the identifier or keyword
            string text = _source.Substring(_start, _current - _start);

            // Check if it's a keyword
            TokenTypes type = _keywords.ContainsKey(text) ? _keywords[text] : TokenTypes.IDENTIFIER;

            AddToken(type);
        }

        // Helper methods

        // Advance the current position and return the current character
        private char Advance()
        {
            _column++;
            return _source[_current++];
        }

        // Check if the current character matches the expected character
        private bool Match(char expected)
        {
            if (IsAtEnd() || _source[_current] != expected)
                return false;

            _current++;
            _column++;
            return true;
        }

        // Peek at the current character without advancing
        private char Peek()
        {
            if (IsAtEnd())
                return '\0';
            return _source[_current];
        }

        // Peek at the next character without advancing
        private char PeekNext()
        {
            if (_current + 1 >= _source.Length)
                return '\0';
            return _source[_current + 1];
        }

        // Check if the character is a digit
        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        // Check if the character is alphabetic or underscore
        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z') ||
                   c == '_';
        }

        // Check if the character is alphanumeric or underscore
        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        // Check if we've reached the end of the source code
        private bool IsAtEnd()
        {
            return _current >= _source.Length;
        }

        // Add a token with no literal value
        private void AddToken(TokenTypes type)
        {
            AddToken(type, null);
        }

        // Add a token with a literal value
        private void AddToken(TokenTypes type, object value)
        {
            string lexeme = _source.Substring(_start, _current - _start);
            _tokens.Add(new Token(type, lexeme, value, _line, _start - GetLineStartPosition() + 1));
        }

        // Helper method to get the position of the start of the current line
        private int GetLineStartPosition()
        {
            int pos = _start;
            while (pos > 0 && _source[pos - 1] != '\n')
            {
                pos--;
            }
            return pos;
        }

    }
}
