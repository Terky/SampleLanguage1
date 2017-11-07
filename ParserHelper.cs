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
        public SemanticExepction(string msg) : base(msg) { }
    }
    // Класс глобальных описаний и статических методов
    // для использования различными подсистемами парсера и сканера
    public static class ParserHelper
    {
        private static Stack<SymbolsRecord> stack = new Stack<SymbolsRecord>();

        public static Stack<SymbolsRecord> Stack
        {
            get
            {
                return stack;
            }
           
        }
        
        private static SymbolTable globalTable = new SymbolTable(null);

        public static SymbolTable GlobalTable
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

        public static SymbolTable TopTable()
        {
            return Stack.Peek().TopTable;
        }

        public static SymbolTable BottomTable()
        {
            return Stack.Peek().BottomTable;
        }

        public static SymbolTable SavedTable()
        {
            return Stack.Peek().SavedTable;
        }

        static ParserHelper()
        {
            GlobalTable.Put("int", new TypeSymbol(Symbol.ValueType.INT));
            GlobalTable.Put("double", new TypeSymbol(Symbol.ValueType.DOUBLE));
            GlobalTable.Put("bool", new TypeSymbol(Symbol.ValueType.BOOL));
            GlobalTable.Put("void", new TypeSymbol(Symbol.ValueType.VOID));
        }

        public static void upCast(VarSymbol value, Symbol.ValueType type)
        {
            switch (value.Type)
            {
                case Symbol.ValueType.INT:
                    value.Type = Symbol.ValueType.DOUBLE;
                    value.Value.dValue = value.Value.iValue;
                    break;
                case Symbol.ValueType.DOUBLE:
                    break;
            }
        }
    }
}