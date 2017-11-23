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

        public VarSymbol(bool value)
        {
            Value = new VarValue();
            Type = ValueType.BOOL;
            Value.bValue = value;
        }

        public VarSymbol(int value)
        {
            Value = new VarValue();
            Type = ValueType.INT;
            Value.iValue = value;
        }

        public VarSymbol(double value)
        {
            Value = new VarValue();
            Type = ValueType.DOUBLE;
            Value.dValue = value;
        }

        public VarSymbol(ValueType type, VarValue value)
        {
            Type = type;
            Value = value;
        }

        public VarSymbol(ValueType type)
        {
            Value = new VarValue();
            Type = type;
        }

        public ValueType Type { get; set; }

        public VarValue Value { get; set; }
    }
}
