using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiladorAula
{
    public class Parser
    {
        private readonly List<Token> _tokens;
        private int _current = 0;
        private List<string> _errors = new List<string>();

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        // Parse the entire program
        public List<Stmt> Parse()
        {
            List<Stmt> statements = new List<Stmt>();
            _errors.Clear();

            try
            {
                while (!IsAtEnd())
                {
                    statements.Add(Declaration());
                }
            }
            catch (ParseException ex)
            {
                _errors.Add(ex.Message);
                Synchronize(); // Error recovery
            }

            return statements;
        }

        // Get any parsing errors
        public List<string> GetErrors()
        {
            return _errors;
        }

        // Parse a declaration (variable declaration or statement)
        private Stmt Declaration()
        {
            try
            {
                if (Match(TokenTypes.INT, TokenTypes.STR))
                {
                    return VarDeclaration();
                }

                return Statement();
            }
            catch (ParseException)
            {
                Synchronize();
                return null;
            }
        }
        // Parse a variable declaration
        private Stmt VarDeclaration()
        {
            TokenTypes type = Previous().Type; // INT or STR

            Token name = Consume(TokenTypes.IDENTIFIER, "Expect variable name.");

            Expr initializer = null;
            if (Match(TokenTypes.ASSIGN))
            {
                initializer = Expression();
            }

            Consume(TokenTypes.SEMICOLON, "Expect ';' after variable declaration.");
            return new VarDeclarationStmt(name, type, initializer);
        }

        // Parse a statement
        private Stmt Statement()
        {
            if (Match(TokenTypes.PRINT)) return PrintStatement();
            if (Match(TokenTypes.IF)) return IfStatement();
            if (Match(TokenTypes.WHILE)) return WhileStatement();
            if (Match(TokenTypes.LEFT_BRACE)) return new BlockStmt(Block());

            return ExpressionStatement();
        }

        // Parse a print statement
        private Stmt PrintStatement()
        {
            Expr value = Expression();
            Consume(TokenTypes.SEMICOLON, "Expect ';' after value.");
            return new PrintStmt(value);
        }

        // Parse an if statement
        private Stmt IfStatement()
        {
            Consume(TokenTypes.LEFT_PAREN, "Expect '(' after 'if'.");
            Expr condition = Expression();
            Consume(TokenTypes.RIGHT_PAREN, "Expect ')' after if condition.");

            Stmt thenBranch = Statement();
            Stmt elseBranch = null;
            if (Match(TokenTypes.ELSE))
            {
                elseBranch = Statement();
            }

            return new IfStmt(condition, thenBranch, elseBranch);
        }


        // Parse a while statement
        private Stmt WhileStatement()
        {
            Consume(TokenTypes.LEFT_PAREN, "Expect '(' after 'while'.");
            Expr condition = Expression();
            Consume(TokenTypes.RIGHT_PAREN, "Expect ')' after condition.");
            Stmt body = Statement();

            return new WhileStmt(condition, body);
        }

        // Parse a block of statements
        private List<Stmt> Block()
        {
            List<Stmt> statements = new List<Stmt>();

            while (!Check(TokenTypes.RIGHT_BRACE) && !IsAtEnd())
            {
                statements.Add(Declaration());
            }

            Consume(TokenTypes.RIGHT_BRACE, "Expect '}' after block.");
            return statements;
        }

        // Parse an expression statement
        private Stmt ExpressionStatement()
        {
            Expr expr = Expression();
            Consume(TokenTypes.SEMICOLON, "Expect ';' after expression.");
            return new ExpressionStmt(expr);
        }

        // Parse an expression
        private Expr Expression()
        {
            return Assignment();
        }

        // Parse an assignment expression
        private Expr Assignment()
        {
            Expr expr = Equality();

            if (Match(TokenTypes.ASSIGN))
            {
                Token equals = Previous();
                Expr value = Assignment();

                if (expr is VariableExpr)
                {
                    Token name = ((VariableExpr)expr).Name;
                    return new AssignExpr(name, value);
                }

                Error(equals, "Invalid assignment target.");
            }

            return expr;
        }

        // Parse equality expressions (==, !=)
        private Expr Equality()
        {
            Expr expr = Comparison();

            while (Match(TokenTypes.EQUAL, TokenTypes.NOT_EQUAL))
            {
                Token op = Previous();
                Expr right = Comparison();
                expr = new BinaryExpr(expr, op, right);
            }

            return expr;
        }

        // Parse comparison expressions (<, >, <=, >=)
        private Expr Comparison()
        {
            Expr expr = Term();

            while (Match(TokenTypes.LESS, TokenTypes.GREATER, TokenTypes.LESS_EQUAL, TokenTypes.GREATER_EQUAL))
            {
                Token op = Previous();
                Expr right = Term();
                expr = new BinaryExpr(expr, op, right);
            }

            return expr;
        }

        // Parse term expressions (+, -)
        private Expr Term()
        {
            Expr expr = Factor();

            while (Match(TokenTypes.PLUS, TokenTypes.MINUS))
            {
                Token op = Previous();
                Expr right = Factor();
                expr = new BinaryExpr(expr, op, right);
            }

            return expr;
        }

        // Parse factor expressions (*, /)
        private Expr Factor()
        {
            Expr expr = Primary();

            while (Match(TokenTypes.MULTIPLY, TokenTypes.DIVIDE))
            {
                Token op = Previous();
                Expr right = Primary();
                expr = new BinaryExpr(expr, op, right);
            }

            return expr;
        }

        // Parse primary expressions (literals, identifiers, grouping)
        private Expr Primary()
        {
            if (Match(TokenTypes.INTEGER))
            {
                return new LiteralExpr(Previous().Value, TokenTypes.INTEGER);
            }

            if (Match(TokenTypes.STRING))
            {
                return new LiteralExpr(Previous().Value, TokenTypes.STRING);
            }

            if (Match(TokenTypes.IDENTIFIER))
            {
                return new VariableExpr(Previous());
            }

            if (Match(TokenTypes.LEFT_PAREN))
            {
                Expr expr = Expression();
                Consume(TokenTypes.RIGHT_PAREN, "Expect ')' after expression.");
                return expr;
            }

            throw Error(Peek(), "Expect expression.");
        }

        // Helper methods

        // Consume a token if it matches the expected type
        private Token Consume(TokenTypes type, string message)
        {
            if (Check(type)) return Advance();

            throw Error(Peek(), message);
        }

        // Create a parse error
        private ParseException Error(Token token, string message)
        {
            string errorMsg = $"Error at line {token.Line}, column {token.Column}: {message}";
            _errors.Add(errorMsg);
            return new ParseException(errorMsg);
        }

        // Synchronize after an error
        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().Type == TokenTypes.SEMICOLON) return;

                switch (Peek().Type)
                {
                    case TokenTypes.INT:
                    case TokenTypes.STR:
                    case TokenTypes.IF:
                    case TokenTypes.WHILE:
                    case TokenTypes.PRINT:
                        return;
                }

                Advance();
            }
        }

        // Check if the current token matches any of the given types
        private bool Match(params TokenTypes[] types)
        {
            foreach (var type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        // Check if the current token is of the given type
        private bool Check(TokenTypes type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }

        // Advance to the next token
        private Token Advance()
        {
            if (!IsAtEnd()) _current++;
            return Previous();
        }

        // Check if we've reached the end of the token stream
        private bool IsAtEnd()
        {
            return Peek().Type == TokenTypes.EOF;
        }

        // Get the current token
        private Token Peek()
        {
            return _tokens[_current];
        }

        // Get the previous token
        private Token Previous()
        {
            return _tokens[_current - 1];
        }
    }

    // Custom exception for parsing errors
    public class ParseException : Exception
    {
        public ParseException(string message) : base(message) { }
    }
}

