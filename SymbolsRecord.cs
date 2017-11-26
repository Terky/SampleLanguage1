using SimpleParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    public class SymbolsRecord
    {
        public SymbolTable TopTable { get; set; }
        public SymbolTable BottomTable { get; set; }

        public SymbolsRecord()
        {
            BottomTable = ParserHelper.GlobalTable;
            TopTable = new SymbolTable(BottomTable);
        }
    }
}
