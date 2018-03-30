using System;
using System.IO;
using System.Collections.Generic;
using SimpleScanner;
using SimpleParser;
using SimpleLang;
using Hime.Redist;
using Hime.SDK;
using Hime.SDK.Reflection;
using Hime.SDK.Output;

namespace SimpleCompiler
{
    public class SimpleCompilerMain
    {
        public static void Main()
        {
            //string FileName = @"../../a.txt";
            //try
            //{
            //    string Text = File.ReadAllText(FileName);

            //    Scanner scanner = new Scanner();
            //    scanner.SetSource(Text, 0);

            //    Parser parser = new Parser(scanner);

            //    var b = parser.Parse();
            //    if (!b)
            //        Console.WriteLine("Error"); //Ошибка
            //    else
            //    {
            //        Console.WriteLine("Sytax tree has built"); //Синтаксическое дерево построено
            //        foreach (var st in parser.root.StList)
            //            Console.WriteLine(st);
            //    }
            //    parser.root.Visit(new StaticCheckVisitor());
            //    parser.root.Visit(new ExecutionVisitor());
            //}
            //catch (FileNotFoundException)
            //{
            //    Console.WriteLine("File {0} not found", FileName); //Файл {0} не найден
            //}
            //catch (LexException e)
            //{
            //    Console.WriteLine("Lexical error. " + e.Message); //Лексическая ошибка
            //}
            //catch (SyntaxException e)
            //{
            //    Console.WriteLine("Syntax error. " + e.Message); //Синтаксическая ошибка
            //}
            //catch (IncompatibleTypesException e)
            //{
            //    Console.WriteLine("Incompatible types " + e.Message);
            //}
            //catch (SemanticExepction e)
            //{
            //    Console.WriteLine("Semantic error. " + e.Message);
            //}
            //Console.ReadLine();
            // Define an inline grammar
            string fileName = @"../../SimpleLangGram1.gram";
            // Setup the compilation task
            CompilationTask task = new CompilationTask();
            task.AddInputFile(fileName);
            task.Mode = Mode.Assembly;
            task.Namespace = "SimpleLanguage.Grammar";
            task.CodeAccess = Modifier.Public;
            // Execute
            task.Execute();
            // Load the generated assembly
            AssemblyReflection assembly = new AssemblyReflection(@"../../SimpleLangGram.dll");

            // define some input string
            string input = System.IO.File.ReadAllText(@"../../a.txt");
            Console.WriteLine(input);
            Console.WriteLine();
            // dynamically instantiate the parser
            var parser = assembly.GetParser(input);
            // parse the input
            ParseResult result = parser.Parse();
            if (!result.IsSuccess)
            {
                foreach (var err in result.Errors)
                {
                    Console.WriteLine(err.ToString());
                }
            }
            else
            {
                // get the produced AST root
                ASTNode root = result.Root;
                Print(root, new bool[] { });
            }
            Console.ReadLine();
        }

        private static void Print(ASTNode node, bool[] crossings)
        {
            for (int i = 0; i < crossings.Length - 1; i++)
                Console.Write(crossings[i] ? "|   " : "    ");
            if (crossings.Length > 0)
                Console.Write("+-> ");
            Console.WriteLine(node.ToString());
            for (int i = 0; i != node.Children.Count; i++)
            {
                bool[] childCrossings = new bool[crossings.Length + 1];
                Array.Copy(crossings, childCrossings, crossings.Length);
                childCrossings[childCrossings.Length - 1] = (i < node.Children.Count - 1);
                Print(node.Children[i], childCrossings);
            }
        }

    }
}
