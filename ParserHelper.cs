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
    public class SemanticExepction : Exception
    {
        public SemanticExepction(string msg) : base(msg) { }
    }
    // Класс глобальных описаний и статических методов
    // для использования различными подсистемами парсера и сканера
    public static class ParserHelper
    {

        public const string RESULT = "@result";
        public static SymbolTable savedTable = null;
        public static SymbolTable topTable = null;
        public static SymbolTable globalTable = null;

        static ParserHelper()
        {
            topTable = new SymbolTable(null);
            TypeSymbol intSym = new TypeSymbol();
            intSym.Value = Symbol.ValueType.INT;
            topTable.Put("int", intSym);
            TypeSymbol doubleSym = new TypeSymbol();
            doubleSym.Value = Symbol.ValueType.DOUBLE;
            topTable.Put("double", doubleSym);
            TypeSymbol boolSym = new TypeSymbol();
            boolSym.Value = Symbol.ValueType.BOOL;
            topTable.Put("bool", boolSym);
            TypeSymbol voidSym = new TypeSymbol();
            voidSym.Value = Symbol.ValueType.VOID;
            topTable.Put("void", voidSym);
            globalTable = topTable;
            globalTable.Put(RESULT, new VarSymbol());
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