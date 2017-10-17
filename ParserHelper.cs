using SimpleLang;
using System;

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
    // Класс глобальных описаний и статических методов
    // для использования различными подсистемами парсера и сканера
    public static class ParserHelper
    {

        public static SymbolTable saved;

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