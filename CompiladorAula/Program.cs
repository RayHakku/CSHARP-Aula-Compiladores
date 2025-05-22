using System;
using System.Collections.Generic;
using CompiladorAula;



class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("SimpleLang Compiler");
        Console.WriteLine("------------------");

        if (args.Length == 0)
        {
            Console.WriteLine("Usage: SimpleLang <source-file>");
            return;
        }

        string sourceFile = args[0];
        string sourceCode;

        try
        {
            sourceCode = System.IO.File.ReadAllText(sourceFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
            return;
        }

        Compiler compiler = new Compiler(sourceCode);
        bool success = compiler.Compile();

        if (success)
        {
            Console.WriteLine("Compilation successful!");
        }
        else
        {
            Console.WriteLine("Compilation failed with errors:");
            foreach (string error in compiler.GetErrors())
            {
                Console.WriteLine($"  - {error}");
            }
        }
    }
}

