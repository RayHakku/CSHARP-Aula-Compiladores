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
            foreach (Token token in tokens)
            {
                Console.WriteLine(token);
            }
        }
    }
