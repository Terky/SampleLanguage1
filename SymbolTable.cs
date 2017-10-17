using System.Collections.Generic;

namespace SimpleLang
{
    public class SymbolTable
    {
        private Dictionary<string, Symbol> table;
        protected SymbolTable prev;
        
        public SymbolTable(SymbolTable prev)
        {
            table = new Dictionary<string, Symbol>();
            this.prev = prev;
        }

        public void Put(string id, Symbol sym)
        {
            table.Add(id, sym);
        }

        public Symbol Get(string id)
        {
            for (SymbolTable ct = this; ct != null; ct = ct.prev)
            {
                Symbol found = (Symbol)ct.table[id];
                if (found != null)
                    return found;
            }
            return null;
        }
    }
}
