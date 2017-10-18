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

        public Symbol Get(string key)
        {
            for (SymbolTable st = this; st != null; st = st.prev)
            {
                Symbol found;
                bool suc = st.table.TryGetValue(key, out found);
                if (!suc && st.prev == null)
                {
                    System.Console.WriteLine("SYMBOL {0} NOT FOUND", key);

                }
                if (found!=null)
                {
                    return found;
                }
            }
            return null;
        }
    }
}
