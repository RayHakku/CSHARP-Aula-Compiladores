using System;
using System.Collections.Generic;
using CompiladorAula;



class Program
{
    static void Main(string[] args)
    {
        // Test source code
        string source = @"
            int x = 10;
            string message = ""Hello, World!"";
            
            if (x > 5) {
                print(message);
            } else {
                print(""x is too small"");
            }
            
            while (x > 0) {
                x = x - 1;
                print(x);
            }
        ";

        Console.WriteLine("====================== CÓDIGO FONTE ======================");
        Console.WriteLine(source);
        Console.WriteLine("=========================================================\n");

        // Create a lexer instance
        Lexer lexer = new Lexer(source);

        // Scan tokens
        List<Token> tokens = lexer.ScanTokens();

        // Print tokens in tabular format
        PrintTokens(tokens);

        // Create a parser and generate AST
        Parser parser = new Parser(tokens);
        List<Stmt> statements = parser.Parse();

        // Check for parsing errors
        List<string> errors = parser.GetErrors();
        if (errors.Count > 0)
        {
            Console.WriteLine("\n====================== ERROS DE PARSING ======================");
            foreach (string error in errors)
            {
                Console.WriteLine($"  {error}");
            }
            Console.WriteLine("==============================================================");
            return;
        }

        // Print AST details
        PrintStatements(statements);
    }

    // Helper method to print tokens in a structured table
    static void PrintTokens(List<Token> tokens)
    {
        Console.WriteLine("\n====================== TOKENS ======================");

        // Definir larguras das colunas
        const int typeWidth = 15;
        const int lexemeWidth = 20;
        const int valueWidth = 20;
        const int positionWidth = 15;

        // Imprimir cabeçalho
        Console.WriteLine($"{"TIPO",-typeWidth} | {"LEXEMA",-lexemeWidth} | {"VALOR",-valueWidth} | {"POSIÇÃO",-positionWidth}");
        Console.WriteLine(new string('-', typeWidth + lexemeWidth + valueWidth + positionWidth + 9));

        foreach (Token token in tokens)
        {
            string valueStr = token.Value?.ToString() ?? "null";
            if (token.Type == TokenTypes.STRING)
            {
                valueStr = $"\"{valueStr}\"";
            }

            string position = $"Ln {token.Line}, Col {token.Column}";

            // Truncar strings se forem muito longas
            string lexeme = token.Lexeme.Length > lexemeWidth - 3
                ? token.Lexeme.Substring(0, lexemeWidth - 3) + "..."
                : token.Lexeme;

            valueStr = valueStr.Length > valueWidth - 3
                ? valueStr.Substring(0, valueWidth - 3) + "..."
                : valueStr;

            Console.WriteLine($"{token.Type,-typeWidth} | {lexeme,-lexemeWidth} | {valueStr,-valueWidth} | {position,-positionWidth}");
        }

        Console.WriteLine("====================================================\n");
    }

    // Helper method to print statements with indentation
    static void PrintStatements(List<Stmt> statements)
    {
        Console.WriteLine("\n====================== AST (ÁRVORE SINTÁTICA) ======================");
        Console.WriteLine($"Parsing bem-sucedido! AST criada com {statements.Count} statements.\n");

        int indentLevel = 0;
        foreach (Stmt stmt in statements)
        {
            PrintStatement(stmt, indentLevel);
        }

        Console.WriteLine("===================================================================");
    }

    // Helper method to print a statement with indentation
    static void PrintStatement(Stmt stmt, int indentLevel)
    {
        string indent = new string(' ', indentLevel * 2);

        if (stmt is VarDeclarationStmt varDecl)
        {
            Console.WriteLine($"{indent}Declaração de Variável: {varDecl.Type} {varDecl.Name.Lexeme}");
            if (varDecl.Initializer != null)
            {
                Console.WriteLine($"{indent}  Inicializada com: ");
                PrintExpression(varDecl.Initializer, indentLevel + 2);
            }
        }
        else if (stmt is PrintStmt printStmt)
        {
            Console.WriteLine($"{indent}Print Statement:");
            PrintExpression(printStmt.Expression, indentLevel + 1);
        }
        else if (stmt is IfStmt ifStmt)
        {
            Console.WriteLine($"{indent}If Statement:");
            Console.WriteLine($"{indent}  Condição:");
            PrintExpression(ifStmt.Condition, indentLevel + 2);
            Console.WriteLine($"{indent}  Then Branch:");
            PrintStatement(ifStmt.ThenBranch, indentLevel + 2);

            if (ifStmt.ElseBranch != null)
            {
                Console.WriteLine($"{indent}  Else Branch:");
                PrintStatement(ifStmt.ElseBranch, indentLevel + 2);
            }
        }
        else if (stmt is WhileStmt whileStmt)
        {
            Console.WriteLine($"{indent}While Statement:");
            Console.WriteLine($"{indent}  Condição:");
            PrintExpression(whileStmt.Condition, indentLevel + 2);
            Console.WriteLine($"{indent}  Body:");
            PrintStatement(whileStmt.Body, indentLevel + 2);
        }
        else if (stmt is BlockStmt blockStmt)
        {
            Console.WriteLine($"{indent}Block Statement:");
            foreach (Stmt s in blockStmt.Statements)
            {
                PrintStatement(s, indentLevel + 1);
            }
        }
        else if (stmt is ExpressionStmt exprStmt)
        {
            Console.WriteLine($"{indent}Expression Statement:");
            PrintExpression(exprStmt.Expression, indentLevel + 1);
        }
        else
        {
            Console.WriteLine($"{indent}Unknown Statement: {stmt.GetType().Name}");
        }
    }

    // Helper method to print an expression with indentation
    static void PrintExpression(Expr expr, int indentLevel)
    {
        string indent = new string(' ', indentLevel * 2);

        if (expr is BinaryExpr binary)
        {
            Console.WriteLine($"{indent}Expressão Binária: Operador '{binary.Operator.Lexeme}'");
            Console.WriteLine($"{indent}  Lado Esquerdo:");
            PrintExpression(binary.Left, indentLevel + 2);
            Console.WriteLine($"{indent}  Lado Direito:");
            PrintExpression(binary.Right, indentLevel + 2);
        }
        else if (expr is LiteralExpr literal)
        {
            string valueStr = literal.Value?.ToString() ?? "null";
            if (literal.Type == TokenTypes.STRING)
            {
                valueStr = $"\"{valueStr}\"";
            }
            Console.WriteLine($"{indent}Literal: {valueStr} (Tipo: {literal.Type})");
        }
        else if (expr is VariableExpr variable)
        {
            Console.WriteLine($"{indent}Variável: {variable.Name.Lexeme}");
        }
        else if (expr is AssignExpr assign)
        {
            Console.WriteLine($"{indent}Atribuição: {assign.Name.Lexeme} =");
            PrintExpression(assign.Value, indentLevel + 1);
        }
        else
        {
            Console.WriteLine($"{indent}Expressão Desconhecida: {expr.GetType().Name}");
        }
    }
}
