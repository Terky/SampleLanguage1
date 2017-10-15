using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    public class Symbol
    {
        public enum SymbolType { tint, tdouble };
        
        public SymbolType Type { get; set; }

    }
}
