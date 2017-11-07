using ProgramTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    public class FunSymbol : Symbol
    {

        public FunSymbol() { }

        public FunSymbol(ValueType type, FunNode address)
        {
            Type = type;
            Address = address;
        }

        public ValueType Type { get; set; }

        public FunNode Address { get; set; }


    }
}
