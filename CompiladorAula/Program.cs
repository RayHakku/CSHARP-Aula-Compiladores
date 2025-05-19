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

            // Create a lexer instance
            Lexer lexer = new Lexer(source);

            // Scan tokens
            List<Token> tokens = lexer.ScanTokens();

            // Print tokens
            //foreach (Token token in tokens)
            //{
            //    Console.WriteLine(token);
            //}

            // Create a parser and generate AST
            Parser parser = new Parser(tokens);
            List<Stmt> statements = parser.Parse();
            
            // Check for parsing errors
            List<string> errors = parser.GetErrors();
            if (errors.Count > 0)
            {
                foreach (string error in errors)
                {
                    Console.WriteLine(error);
                }
                return;
            }
            
            // Print AST (simple representation)
            Console.WriteLine("Parsing successful! AST created with " + statements.Count + " statements.");
            
            // To properly display the AST, we would implement a visitor pattern
            // For now, we'll just print the type of each statement
            foreach (Stmt stmt in statements)
            {
                Console.WriteLine("Statement type: " + stmt.GetType().Name);
            }


        }
    }
