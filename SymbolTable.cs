using System.Collections.Generic;

namespace SimpleLang
{
    public class SymbolTable
    {
        public const string RESULT = "@result";

        private Dictionary<string, Symbol> table;
        protected SymbolTable prev;
                
        public SymbolTable(SymbolTable prev)
        {
            table = new Dictionary<string, Symbol>();
            this.prev = prev;
        }

        public bool Contains(string key)
        {
            for (SymbolTable st = this; st != null; st = st.prev)
            {
                bool res = st.table.ContainsKey(key);
                if (res)
                {
                    return true;
                }
            }
            return false;
        }

        public void Put(string key, Symbol sym)
        {
            if (Contains(key))
            {
                throw new SimpleParser.SemanticExepction("Повторное обьявление переменной " + key);
            }
            table.Add(key, sym);
        }

        public Symbol Get(string key)
        {
            for (SymbolTable st = this; st != null; st = st.prev)
            {
                Symbol found;
                bool suc = st.table.TryGetValue(key, out found);
                if (!suc && st.prev == null)
                {
                    throw new SimpleParser.SemanticExepction("Variable not in scope: " + key);

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
