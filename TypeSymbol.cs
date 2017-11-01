using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    public class TypeSymbol: Symbol
    {
        public TypeSymbol(ValueType value)
        {
            Value = value;
        }

        public ValueType Value { get; set; }
    }

}
