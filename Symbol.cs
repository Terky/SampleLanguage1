using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    public abstract class Symbol
    {
        public enum ValueType
        {
            INT,
            DOUBLE,
            BOOL,
            VOID,
            UNKNOWN
        };
        
       

        //public SymbolType? ParseType(String str)
        //{
        //    switch (str) {
        //        case "int":
        //            return SymbolType.INT;
        //        case "double":
        //            return SymbolType.DOUBLE;
        //        case "bool":
        //            return SymbolType.BOOL;
        //        default:
        //            return SymbolType.UNKNOWN;

        //    }
        //}
    }
}
