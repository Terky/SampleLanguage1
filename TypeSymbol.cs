using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    public class TypeSymbol: Symbol
    {
        public ValueType Value { get; set; }
    }

}
