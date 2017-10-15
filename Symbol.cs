using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    public class Symbol
    {
        public enum SymbolType
        {
            INT,
            DOUBLE,
            BOOL,
            UNKNOWN
        };
        
        public SymbolType Type { get; set; }

        public SymbolType ParseType(String str)
        {
            switch (str) {
                case "int":
                    return SymbolType.INT;
                case "double":
                    return SymbolType.DOUBLE;
                case "bool":
                    return SymbolType.BOOL;
                default:
                    return SymbolType.UNKNOWN;

            }
        }
    }
}
