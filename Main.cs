using System;
using System.IO;
using System.Collections.Generic;
using SimpleScanner;
using SimpleParser;

namespace SimpleCompiler
{
    public class SimpleCompilerMain
    {
        public static void Main()
        {
            string FileName = @"..\..\a.txt";
            try
            {
                string Text = File.ReadAllText(FileName);

                Scanner scanner = new Scanner();
                scanner.SetSource(Text, 0);

                Parser parser = new Parser(scanner);

                var b = parser.Parse();
                if (!b)
                    Console.WriteLine("ЕГГОГ"); //Ошибка
                else
                {
                    Console.WriteLine("Sytax tree has built"); //Синтаксическое дерево построено
                    //foreach (var st in parser.root.StList)
                    //Console.WriteLine(st);
                }
                //root.exec();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Файл {0} не найден", FileName);
            }
            catch (LexException e)
            {
                Console.WriteLine("Lexical error. " + e.Message); //Лексическая ошибка
            }
            catch (SyntaxException e)
            {
                Console.WriteLine("Syntax error. " + e.Message); //Синтаксическая ошибка
            }

            Console.ReadLine();
        }

    }
}
