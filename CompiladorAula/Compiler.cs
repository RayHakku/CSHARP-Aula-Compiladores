using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiladorAula
{
        public class Compiler
        {
            private readonly string _sourceCode;
            private readonly List<string> _errors = new List<string>();

            public Compiler(string sourceCode)
            {
                _sourceCode = sourceCode;
            }

            public bool Compile()
            {
                _errors.Clear();

                // Phase 1: Lexical Analysis
                Lexer lexer = new Lexer(_sourceCode);
                List<Token> tokens;

                try
                {
                    tokens = lexer.ScanTokens();
                }
                catch (Exception ex)
                {
                    _errors.Add($"Lexical error: {ex.Message}");
                    return false;
                }

                // Phase 2: Syntax Analysis
                Parser parser = new Parser(tokens);
                List<Stmt> statements;

                try
                {
                    statements = parser.Parse();
                    List<string> parseErrors = parser.GetErrors();

                    if (parseErrors.Count > 0)
                    {
                        _errors.AddRange(parseErrors);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _errors.Add($"Syntax error: {ex.Message}");
                    return false;
                }

                // Phase 3: Semantic Analysis
                SemanticAnalyzer analyzer = new SemanticAnalyzer();

                try
                {
                    bool valid = analyzer.Analyze(statements);

                    if (!valid)
                    {
                        _errors.AddRange(analyzer.GetErrors());
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _errors.Add($"Semantic error: {ex.Message}");
                    return false;
                }

                // Compilation successful
                return true;
            }

            public List<string> GetErrors()
            {
                return _errors;
            }
        }
    
}
