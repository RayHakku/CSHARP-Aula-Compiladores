using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiladorAula
{
    public class SemanticAnalyzer : IAstVisitor<TokenTypes>
    {
        private readonly Dictionary<string, Symbol> _symbolTable = new Dictionary<string, Symbol>();
        private readonly List<string> _errors = new List<string>();

        // Analyze the AST for semantic errors
        public bool Analyze(List<Stmt> statements)
        {
            _errors.Clear();
            _symbolTable.Clear();

            try
            {
                foreach (var stmt in statements)
                {
                    AnalyzeStatement(stmt);
                }

                return _errors.Count == 0;
            }
            catch (SemanticException ex)
            {
                _errors.Add(ex.Message);
                return false;
            }
        }

        // Get semantic errors
        public List<string> GetErrors()
        {
            return _errors;
        }

        // Analyze a statement
        private void AnalyzeStatement(Stmt stmt)
        {
            if (stmt is VarDeclarationStmt varDecl)
            {
                AnalyzeVarDeclaration(varDecl);
            }
            else if (stmt is BlockStmt blockStmt)
            {
                foreach (var s in blockStmt.Statements)
                {
                    AnalyzeStatement(s);
                }
            }
            else if (stmt is IfStmt ifStmt)
            {
                // Check if condition is boolean
                TokenTypes conditionType = EvaluateExpr(ifStmt.Condition);
                if (conditionType != TokenTypes.UNKNOWN && conditionType != TokenTypes.INTEGER)  // Using INTEGER as boolean in our simple language
                {
                    ReportError(ifStmt.Condition, "Condition must be a boolean expression.");
                }

                AnalyzeStatement(ifStmt.ThenBranch);
                if (ifStmt.ElseBranch != null)
                {
                    AnalyzeStatement(ifStmt.ElseBranch);
                }
            }
            else if (stmt is WhileStmt whileStmt)
            {
                // Check if condition is boolean
                TokenTypes conditionType = EvaluateExpr(whileStmt.Condition);
                if (conditionType != TokenTypes.UNKNOWN && conditionType != TokenTypes.INTEGER)  // Using INTEGER as boolean in our simple language
                {
                    ReportError(whileStmt.Condition, "Condition must be a boolean expression.");
                }

                AnalyzeStatement(whileStmt.Body);
            }
            else if (stmt is PrintStmt printStmt)
            {
                // Any expression can be printed
                EvaluateExpr(printStmt.Expression);
            }
            else if (stmt is ExpressionStmt exprStmt)
            {
                EvaluateExpr(exprStmt.Expression);
            }
        }

        // Analyze variable declaration
        private void AnalyzeVarDeclaration(VarDeclarationStmt varDecl)
        {
            string name = varDecl.Name.Lexeme;

            // Check if variable already exists
            if (_symbolTable.ContainsKey(name))
            {
                ReportError(varDecl.Name, $"Variable '{name}' already declared.");
                return;
            }

            TokenTypes type = varDecl.Type;
            bool initialized = varDecl.Initializer != null;

            // Add to symbol table
            _symbolTable.Add(name, new Symbol(name, type, initialized));

            // Check initializer
            if (initialized)
            {
                TokenTypes initializerType = EvaluateExpr(varDecl.Initializer);

                // Check type compatibility
                if (initializerType != TokenTypes.UNKNOWN && initializerType != type)
                {
                    ReportError(varDecl.Initializer, $"Cannot assign {initializerType} to {type} variable '{name}'.");
                }
            }
        }

        // Evaluate an expression and return its type
        private TokenTypes EvaluateExpr(Expr expr)
        {
            return expr.Accept(this);
        }
        public TokenTypes VisitAsignExpr(AssignExpr expr)
        {
            string name = expr.Name.Lexeme;

            // Check if variable is declared
            if (!_symbolTable.ContainsKey(name))
            {
                ReportError(expr.Name, $"Variable '{name}' is not declared.");
                return TokenTypes.UNKNOWN;
            }

            Symbol symbol = _symbolTable[name];
            TokenTypes valueType = expr.Value.Accept(this);

            // Check type compatibility
            if (valueType != TokenTypes.UNKNOWN && valueType != symbol.Type)
            {
                ReportError(expr, $"Cannot assign {valueType} to {symbol.Type} variable '{name}'.");
            }

            // Mark as initialized
            symbol.Initialized = true;

            return symbol.Type;
        }

        public TokenTypes VisitBinaryExpr(BinaryExpr expr)
        {
            TokenTypes leftType = expr.Left.Accept(this);
            TokenTypes rightType = expr.Right.Accept(this);

            // Handle unknown types
            if (leftType == TokenTypes.UNKNOWN || rightType == TokenTypes.UNKNOWN)
            {
                return TokenTypes.UNKNOWN;
            }

            // Check operator
            switch (expr.Operator.Type)
            {
                case TokenTypes.PLUS:
                    // String concatenation or numeric addition
                    if (leftType == TokenTypes.STR && rightType == TokenTypes.STR)
                    {
                        return TokenTypes.STR;
                    }
                    else if (leftType == TokenTypes.INT && rightType == TokenTypes.INT)
                    {
                        return TokenTypes.INT;
                    }
                    else
                    {
                        ReportError(expr, $"Cannot apply '+' to {leftType} and {rightType}.");
                        return TokenTypes.UNKNOWN;
                    }

                case TokenTypes.MINUS:
                case TokenTypes.MULTIPLY:
                case TokenTypes.DIVIDE:
                    // Numeric operations
                    if (leftType == TokenTypes.INT && rightType == TokenTypes.INT)
                    {
                        return TokenTypes.INT;
                    }
                    else
                    {
                        ReportError(expr, $"Cannot apply '{expr.Operator.Lexeme}' to {leftType} and {rightType}.");
                        return TokenTypes.UNKNOWN;
                    }

                case TokenTypes.EQUAL:
                case TokenTypes.NOT_EQUAL:
                    // Equality operators
                    if (leftType == rightType)
                    {
                        return TokenTypes.INT; // Using INTEGER as boolean
                    }
                    else
                    {
                        ReportError(expr, $"Cannot compare {leftType} and {rightType}.");
                        return TokenTypes.UNKNOWN;
                    }

                case TokenTypes.LESS:
                case TokenTypes.GREATER:
                case TokenTypes.LESS_EQUAL:
                case TokenTypes.GREATER_EQUAL:
                    // Comparison operators (only for numbers)
                    if (leftType == TokenTypes.INT && rightType == TokenTypes.INT)
                    {
                        return TokenTypes.INT; // Using INTEGER as boolean
                    }
                    else
                    {
                        ReportError(expr, $"Cannot apply '{expr.Operator.Lexeme}' to {leftType} and {rightType}.");
                        return TokenTypes.UNKNOWN;
                    }

                default:
                    return TokenTypes.UNKNOWN;
            }
        }

        public TokenTypes VisitBlockStmt(BlockStmt stmt)
        {
            throw new NotImplementedException();
        }

        public TokenTypes VisitExpressionStmt(ExpressionStmt stmt)
        {
            throw new NotImplementedException();
        }

        public TokenTypes VisitIfStmt(IfStmt stmt)
        {
            throw new NotImplementedException();
        }

        public TokenTypes VisitLiteralExpr(LiteralExpr expr)
        {
            return expr.Type;
        }

        public TokenTypes VisitPrintStmt(PrintStmt stmt)
        {
            throw new NotImplementedException();
        }

        public TokenTypes VisitVarDeclarationStmt(VarDeclarationStmt stmt)
        {
            throw new NotImplementedException();
        }

        public TokenTypes VisitVariableExpr(VariableExpr expr)
        {
            string name = expr.Name.Lexeme;

            // Check if variable is declared
            if (!_symbolTable.ContainsKey(name))
            {
                ReportError(expr.Name, $"Variable '{name}' is not declared.");
                return TokenTypes.UNKNOWN;
            }

            Symbol symbol = _symbolTable[name];

            // Check if variable is initialized
            if (!symbol.Initialized)
            {
                ReportError(expr.Name, $"Variable '{name}' might not have been initialized.");
                // We don't return unknown because we know the declared type
            }

            return symbol.Type;
        }

        public TokenTypes VisitWhileStmt(WhileStmt stmt)
        {
            throw new NotImplementedException();
        }

        private void ReportError(object node, string message)
        {
            // For simplicity, we just add the error message
            // In a real compiler, we would also add the line and column information
            _errors.Add(message);
        }
    }
}
