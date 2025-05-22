using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiladorAula
{
    public enum TokenTypes
    {
        // Special tokens
        EOF,        // End of file
        UNKNOWN,    // Unknown token

        // Literals
        INTEGER,    // Integer literal
        STRING,     // String literal
        IDENTIFIER, // Variable or function name
        BOOLEAN,

        // Keywords
        VAR,        // 'var' keyword
        IF,         // 'if' keyword
        ELSE,       // 'else' keyword
        WHILE,      // 'while' keyword
        PRINT,      // 'print' keyword
        INT,        // 'int' type
        STR,        // 'string' type
        BOOL,

        // Operators
        PLUS,       // '+'
        MINUS,      // '-'
        MULTIPLY,   // '*'
        DIVIDE,     // '/'
        ASSIGN,     // '='

        // Comparison operators
        EQUAL,      // '=='
        NOT_EQUAL,  // '!='
        LESS,       // '<'
        GREATER,    // '>'
        LESS_EQUAL, // '<='
        GREATER_EQUAL, // '>='

        // Delimiters
        SEMICOLON,  // ';'
        COMMA,      // ','
        LEFT_PAREN, // '('
        RIGHT_PAREN,// ')'
        LEFT_BRACE, // '{'
        RIGHT_BRACE // '}'
    }
}
