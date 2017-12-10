using ProgramTree;
using SimpleLang;
using System;
using System.Collections.Generic;

namespace SimpleParser
{
    public class LexException : Exception
    {
        public LexException(string msg) : base(msg) { }
    }

    public class SyntaxException : Exception
    {
        public SyntaxException(string msg) : base(msg) { }
    }

    public class SemanticExepction : Exception
    {
        public SemanticExepction(string msg, Node node)
            : base(msg + ". Строка " + node.LexLoc.StartLine + ", столбец " + node.LexLoc.StartColumn) { }

        public SemanticExepction(string msg) : base(msg) { }
    }

    public class IncompatibleTypesException: SemanticExepction
    {
        public IncompatibleTypesException(string msg, Node node) : base(msg, node) { }
    }

    // Класс глобальных описаний и статических методов
    // для использования различными подсистемами парсера и сканера
    public static class ParserHelper
    {
        private static Stack<SymbolsRecord> stack = new Stack<SymbolsRecord>();

        private static Stack<SymbolTable> savedTables = new Stack<SymbolTable>();

        public static Stack<SymbolsRecord> Stack
        {
            get
            {
                return stack;
            }
           
        }
        
        private static SymbolTable globalTable = new SymbolTable(null);

        public static SymbolTable GlobalScope
        {
            get
            {
                return globalTable;
            }

            set
            {
                globalTable = value;
            }
        }

        public static SymbolTable CurrentScope()
        {
            return Stack.Peek().TopTable;
        }

        //public static SymbolTable BottomTable()
        //{
        //    return Stack.Peek().BottomTable;
        //}

        static ParserHelper()
        {
            GlobalScope.Put("int", new TypeSymbol(Symbol.ValueType.INT));
            GlobalScope.Put("double", new TypeSymbol(Symbol.ValueType.DOUBLE));
            GlobalScope.Put("bool", new TypeSymbol(Symbol.ValueType.BOOL));
            GlobalScope.Put("void", new TypeSymbol(Symbol.ValueType.VOID));
        }

        public static VarSymbol upCast(VarSymbol value, Symbol.ValueType type)
        {
            VarSymbol res = new VarSymbol();
            switch (value.Type)
            {
                case Symbol.ValueType.INT:
                    res.Type = Symbol.ValueType.DOUBLE;
                    res.Value.dValue = value.Value.iValue;
                    break;
                case Symbol.ValueType.DOUBLE:
                    res.Type = Symbol.ValueType.DOUBLE;
                    res.Value.dValue = value.Value.dValue;
                    break;
            }
            return res;
        }

        public static void leaveFun()
        {
            Stack.Pop();
        }


        public static void enterFun()
        {
            Stack.Push(new SymbolsRecord());
        }

        public static void enterScope()
        {
            savedTables.Push(CurrentScope());
            Stack.Peek().TopTable = new SymbolTable(CurrentScope());
        }

        public static void leaveScope()
        {
            Stack.Peek().TopTable = savedTables.Pop();
        }
    }
}