using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    public class VarSymbol: Symbol
    {
        public class VarValue
        {
            public double dValue { get; set; }

            public int iValue { get; set; }

            public bool bValue { get; set; }
        }

        public VarSymbol()
        {
            Value = new VarValue();
        }

        public ValueType Type { get; set; }

        public VarValue Value { get; set; }
    }
}
