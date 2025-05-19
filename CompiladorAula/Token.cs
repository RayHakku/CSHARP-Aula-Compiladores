using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiladorAula
{
    public class Token
    {
        public TokenTypes Type { get; }
        public string Lexeme { get; }
        public object Value { get; }
        public int Line { get; }
        public int Column { get; }

        public Token(TokenTypes type, string lexeme, object value, int line, int column)
        {
            Type = type;
            Lexeme = lexeme;
            Value = value;
            Line = line;
            Column = column;
        }

        public override string ToString()
        {
            return $"Token({Type}, '{Lexeme}', {Value}, Line: {Line}, Col: {Column})";
        }
    }
}
