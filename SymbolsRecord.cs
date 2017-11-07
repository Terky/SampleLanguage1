using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    public class SymbolsRecord
    {
        public SymbolTable SavedTable { get; set; }
        public SymbolTable TopTable { get; set; }
        public SymbolTable BottomTable { get; set; }

        public SymbolsRecord()
        {
            TopTable = new SymbolTable(null);
            BottomTable = TopTable;
            BottomTable.Put(SymbolTable.RESULT, new VarSymbol());
        }
    }
}
