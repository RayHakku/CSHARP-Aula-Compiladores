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
                // Verificar a condição
                TokenTypes conditionType = EvaluateExpr(ifStmt.Condition);

                // Verificar se é uma expressão booleana
                if (conditionType != TokenTypes.BOOLEAN && !IsComparisonExpression(ifStmt.Condition))
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
                // Verificar a condição
                TokenTypes conditionType = EvaluateExpr(whileStmt.Condition);

                // Verificar se é uma expressão booleana
                if (conditionType != TokenTypes.BOOLEAN && !IsComparisonExpression(whileStmt.Condition))
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

        private bool IsComparisonExpression(Expr expr)
        {
            if (expr is BinaryExpr binaryExpr)
            {
                switch (binaryExpr.Operator.Type)
                {
                    case TokenTypes.EQUAL:
                    case TokenTypes.NOT_EQUAL:
                    case TokenTypes.LESS:
                    case TokenTypes.GREATER:
                    case TokenTypes.LESS_EQUAL:
                    case TokenTypes.GREATER_EQUAL:
                        return true;
                    default:
                        return false;
                }
            }

            // Podemos também permitir literais booleanos (true/false) se sua linguagem os suportar

            return false; // Outras expressões não são consideradas booleanas
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

                // Correção aqui: Mapeamento de tipos literais para tipos de declaração
                TokenTypes mappedType = MapLiteralToDeclarationType(initializerType);

                // Check type compatibility
                if (initializerType != TokenTypes.UNKNOWN && mappedType != type)
                {
                    ReportError(varDecl.Initializer, $"Cannot assign {initializerType} to {type} variable '{name}'.");
                }
            }
        }

        // Função para verificar compatibilidade de tipos numéricos
        private bool AreNumericTypesCompatible(TokenTypes type1, TokenTypes type2)
        {
            TokenTypes mapped1 = MapLiteralToDeclarationType(type1);
            TokenTypes mapped2 = MapLiteralToDeclarationType(type2);

            return (mapped1 == TokenTypes.INT && mapped2 == TokenTypes.INT) ||
                   (mapped1 == TokenTypes.INTEGER && mapped2 == TokenTypes.INTEGER) ||
                   (mapped1 == TokenTypes.INT && mapped2 == TokenTypes.INTEGER) ||
                   (mapped1 == TokenTypes.INTEGER && mapped2 == TokenTypes.INT);
        }

        // Função para verificar compatibilidade de tipos de string
        private bool AreStringTypesCompatible(TokenTypes type1, TokenTypes type2)
        {
            TokenTypes mapped1 = MapLiteralToDeclarationType(type1);
            TokenTypes mapped2 = MapLiteralToDeclarationType(type2);

            return (mapped1 == TokenTypes.STR && mapped2 == TokenTypes.STR) ||
                   (mapped1 == TokenTypes.STRING && mapped2 == TokenTypes.STRING) ||
                   (mapped1 == TokenTypes.STR && mapped2 == TokenTypes.STRING) ||
                   (mapped1 == TokenTypes.STRING && mapped2 == TokenTypes.STR);
        }

        // Mapeia tipos literais para tipos de declaração
        private TokenTypes MapLiteralToDeclarationType(TokenTypes literalType)
        {
            switch (literalType)
            {
                case TokenTypes.INTEGER:
                    return TokenTypes.INT;
                case TokenTypes.STRING:
                    return TokenTypes.STR;
                case TokenTypes.BOOLEAN:
                    return TokenTypes.BOOL;
                default:
                    return literalType;
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
            if (valueType != TokenTypes.UNKNOWN)
            {
                if (symbol.Type == TokenTypes.INT &&
                    (valueType == TokenTypes.INTEGER || valueType == TokenTypes.INT))
                {
                    // OK - Compatível
                }
                else if (symbol.Type == TokenTypes.STR &&
                        (valueType == TokenTypes.STRING || valueType == TokenTypes.STR))
                {
                    // OK - Compatível
                }
                else
                {
                    ReportError(expr, $"Cannot assign {valueType} to {symbol.Type} variable '{name}'.");
                }
            }

            // Mark as initialized
            symbol.Initialized = true;

            return symbol.Type;
        }

        public TokenTypes VisitBinaryExpr(BinaryExpr expr)
        {
            TokenTypes leftType = expr.Left.Accept(this);
            TokenTypes rightType = expr.Right.Accept(this);

            // Mapeie os tipos antes da comparação
            TokenTypes mappedLeftType = MapLiteralToDeclarationType(leftType);
            TokenTypes mappedRightType = MapLiteralToDeclarationType(rightType);

            // Handle unknown types
            if (mappedLeftType == TokenTypes.UNKNOWN || mappedRightType == TokenTypes.UNKNOWN)
            {
                return TokenTypes.UNKNOWN;
            }

            // Check operator
            switch (expr.Operator.Type)
            {
                case TokenTypes.PLUS:
                    // String concatenation
                    if (AreStringTypesCompatible(leftType, rightType))
                    {
                        return TokenTypes.STRING;
                    }
                    // Numeric addition
                    else if (AreNumericTypesCompatible(leftType, rightType))
                    {
                        return TokenTypes.INTEGER;
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
                    if (AreNumericTypesCompatible(leftType, rightType))
                    {
                        return TokenTypes.INTEGER;
                    }
                    else
                    {
                        ReportError(expr, $"Cannot apply '{expr.Operator.Lexeme}' to {leftType} and {rightType}.");
                        return TokenTypes.UNKNOWN;
                    }

                case TokenTypes.EQUAL:
                case TokenTypes.NOT_EQUAL:
                    // Equality operators - verificar se os tipos são compatíveis
                    if ((AreNumericTypesCompatible(leftType, rightType)) ||
                        (AreStringTypesCompatible(leftType, rightType)))
                    {
                        return TokenTypes.BOOLEAN; // Agora retorna BOOLEAN em vez de INTEGER
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
                    if (AreNumericTypesCompatible(leftType, rightType))
                    {
                        return TokenTypes.BOOLEAN; // Agora retorna BOOLEAN em vez de INTEGER
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
            foreach (var s in stmt.Statements)
            {
                AnalyzeStatement(s);
            }
            return TokenTypes.UNKNOWN; // Statements não têm tipo
        }

        public TokenTypes VisitExpressionStmt(ExpressionStmt stmt)
        {
            return stmt.Expression.Accept(this);
        }

        public TokenTypes VisitIfStmt(IfStmt stmt)
        {
            // Verificar condição
            TokenTypes conditionType = stmt.Condition.Accept(this);
            TokenTypes mappedType = MapLiteralToDeclarationType(conditionType);

            if (mappedType != TokenTypes.UNKNOWN &&
                mappedType != TokenTypes.INT &&
                mappedType != TokenTypes.INTEGER)
            {
                ReportError(stmt.Condition, "Condition must be a boolean expression.");
            }

            // Analisar branches
            AnalyzeStatement(stmt.ThenBranch);
            if (stmt.ElseBranch != null)
            {
                AnalyzeStatement(stmt.ElseBranch);
            }

            return TokenTypes.UNKNOWN; // Statements não têm tipo
        }

        public TokenTypes VisitLiteralExpr(LiteralExpr expr)
        {
            return expr.Type;
        }

        public TokenTypes VisitPrintStmt(PrintStmt stmt)
        {
            // Qualquer expressão pode ser impressa
            stmt.Expression.Accept(this);
            return TokenTypes.UNKNOWN; // Statements não têm tipo
        }

        public TokenTypes VisitVarDeclarationStmt(VarDeclarationStmt stmt)
        {
            AnalyzeVarDeclaration(stmt);
            return TokenTypes.UNKNOWN; // Statements não têm tipo
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
            // Verificar condição
            TokenTypes conditionType = stmt.Condition.Accept(this);
            TokenTypes mappedType = MapLiteralToDeclarationType(conditionType);

            if (mappedType != TokenTypes.UNKNOWN &&
                mappedType != TokenTypes.INT &&
                mappedType != TokenTypes.INTEGER)
            {
                ReportError(stmt.Condition, "Condition must be a boolean expression.");
            }

            // Analisar corpo
            AnalyzeStatement(stmt.Body);

            return TokenTypes.UNKNOWN; // Statements não têm tipo
        }

        private void ReportError(object node, string message)
        {
            // For simplicity, we just add the error message
            // In a real compiler, we would also add the line and column information
            _errors.Add(message);
        }

        private bool AreBooleanTypesCompatible(TokenTypes type1, TokenTypes type2)
        {
            TokenTypes mapped1 = MapLiteralToDeclarationType(type1);
            TokenTypes mapped2 = MapLiteralToDeclarationType(type2);

            return (mapped1 == TokenTypes.BOOL && mapped2 == TokenTypes.BOOL) ||
                   (mapped1 == TokenTypes.BOOLEAN && mapped2 == TokenTypes.BOOLEAN) ||
                   (mapped1 == TokenTypes.BOOL && mapped2 == TokenTypes.BOOLEAN) ||
                   (mapped1 == TokenTypes.BOOLEAN && mapped2 == TokenTypes.BOOL);
        }

    }
}
