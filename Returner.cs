using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    public class Returner
    {
        public VarSymbol Value { get; set; }

        public Returner(VarSymbol value)
        {
            Value = value;
        }

        public Returner()
        {
            Value = new VarSymbol();
        }
    }
}
