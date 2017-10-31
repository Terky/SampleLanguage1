﻿using System;
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
            string FileName = @"..\..\b.txt";
            try
            {
                string Text = File.ReadAllText(FileName);

                Scanner scanner = new Scanner();
                scanner.SetSource(Text, 0);

                Parser parser = new Parser(scanner);

                var b = parser.Parse();
                if (!b)
                    Console.WriteLine("Error"); //Ошибка
                else
                {
                    Console.WriteLine("Sytax tree has built"); //Синтаксическое дерево построено
                    //foreach (var st in parser.root.StList)
                    //Console.WriteLine(st);
                }
                parser.root.Exec();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File {0} not found", FileName); //Файл {0} не найден
            }
            catch (LexException e)
            {
                Console.WriteLine("Lexical error. " + e.Message); //Лексическая ошибка
            }
            catch (SyntaxException e)
            {
                Console.WriteLine("Syntax error. " + e.Message); //Синтаксическая ошибка
            }
            catch (SemanticExepction e)
            {
                Console.WriteLine("Semantic error. " + e.Message);
            }

            Console.ReadLine();
        }

    }
}