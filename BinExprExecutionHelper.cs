using SimpleLang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinExprExecutionHelper
{
    public abstract class BinOp
    {
        public abstract VarSymbol Calculate(VarSymbol left, VarSymbol right);
    }

    public class PlusOp: BinOp
    {
        public override VarSymbol Calculate(VarSymbol left, VarSymbol right)
        {
            VarSymbol result = new VarSymbol();
            result.Type = left.Type;
            if (left.Type == Symbol.ValueType.INT)
            {
                result.Value.iValue = left.Value.iValue + right.Value.iValue;
            }
            else if (left.Type == Symbol.ValueType.DOUBLE)
            {
                result.Value.dValue = left.Value.dValue + right.Value.dValue;
            }
            return result;
        }
    }

    public class MinusOp : BinOp
    {
        public override VarSymbol Calculate(VarSymbol left, VarSymbol right)
        {
            VarSymbol result = new VarSymbol();
            result.Type = left.Type;
            if (left.Type == Symbol.ValueType.INT)
            {
                result.Value.iValue = left.Value.iValue - right.Value.iValue;
            }
            else if (left.Type == Symbol.ValueType.DOUBLE)
            {
                result.Value.dValue = left.Value.dValue - right.Value.dValue;
            }
            return result;
        }
    }

    public class MultOp : BinOp
    {
        public override VarSymbol Calculate(VarSymbol left, VarSymbol right)
        {
            VarSymbol result = new VarSymbol();
            result.Type = left.Type;
            if (left.Type == Symbol.ValueType.INT)
            {
                result.Value.iValue = left.Value.iValue * right.Value.iValue;
            }
            else if (left.Type == Symbol.ValueType.DOUBLE)
            {
                result.Value.dValue = left.Value.dValue * right.Value.dValue;
            }
            return result;
        }
    }

    public class DivOp : BinOp
    {
        public override VarSymbol Calculate(VarSymbol left, VarSymbol right)
        {
            VarSymbol result = new VarSymbol();
            result.Type = left.Type;
            if (left.Type == Symbol.ValueType.INT)
            {
                result.Value.iValue = left.Value.iValue / right.Value.iValue;
            }
            else if (left.Type == Symbol.ValueType.DOUBLE)
            {
                result.Value.dValue = left.Value.dValue / right.Value.dValue;
            }
            return result;
        }
    }

    public class AndOp : BinOp
    {
        public override VarSymbol Calculate(VarSymbol left, VarSymbol right)
        {
            VarSymbol result = new VarSymbol();
            result.Type = Symbol.ValueType.BOOL;
            result.Value.bValue = left.Value.bValue && right.Value.bValue;
            return result;
        }
    }

    public class OrOp : BinOp
    {
        public override VarSymbol Calculate(VarSymbol left, VarSymbol right)
        {
            VarSymbol result = new VarSymbol();
            result.Type = Symbol.ValueType.BOOL;
            result.Value.bValue = left.Value.bValue || right.Value.bValue;
            return result;
        }
    }

    public class GtOp : BinOp
    {
        public override VarSymbol Calculate(VarSymbol left, VarSymbol right)
        {
            VarSymbol result = new VarSymbol();
            result.Type = Symbol.ValueType.BOOL;
            if (left.Type == Symbol.ValueType.INT)
            {
                result.Value.bValue = left.Value.iValue > right.Value.iValue;
            }
            else if (left.Type == Symbol.ValueType.DOUBLE)
            {
                result.Value.bValue = left.Value.dValue > right.Value.dValue;
            }
            return result;
        }
    }

    public class LtOp : BinOp
    {
        public override VarSymbol Calculate(VarSymbol left, VarSymbol right)
        {
            VarSymbol result = new VarSymbol();
            result.Type = Symbol.ValueType.BOOL;
            if (left.Type == Symbol.ValueType.INT)
            {
                result.Value.bValue = left.Value.iValue < right.Value.iValue;
            }
            else if (left.Type == Symbol.ValueType.DOUBLE)
            {
                result.Value.bValue = left.Value.dValue < right.Value.dValue;
            }
            return result;
        }
    }

    public class GetOp : BinOp
    {
        public override VarSymbol Calculate(VarSymbol left, VarSymbol right)
        {
            VarSymbol result = new VarSymbol();
            result.Type = Symbol.ValueType.BOOL;
            if (left.Type == Symbol.ValueType.INT)
            {
                result.Value.bValue = left.Value.iValue >= right.Value.iValue;
            }
            else if (left.Type == Symbol.ValueType.DOUBLE)
            {
                result.Value.bValue = left.Value.dValue >= right.Value.dValue;
            }
            return result;
        }
    }

    public class LetOp : BinOp
    {
        public override VarSymbol Calculate(VarSymbol left, VarSymbol right)
        {
            VarSymbol result = new VarSymbol();
            result.Type = Symbol.ValueType.BOOL;
            if (left.Type == Symbol.ValueType.INT)
            {
                result.Value.bValue = left.Value.iValue <= right.Value.iValue;
            }
            else if (left.Type == Symbol.ValueType.DOUBLE)
            {
                result.Value.bValue = left.Value.dValue <= right.Value.dValue;
            }
            return result;
        }
    }

    public class EqOp : BinOp
    {
        public override VarSymbol Calculate(VarSymbol left, VarSymbol right)
        {
            VarSymbol result = new VarSymbol();
            result.Type = Symbol.ValueType.BOOL;
            if (left.Type == Symbol.ValueType.INT)
            {
                result.Value.bValue = left.Value.iValue == right.Value.iValue;
            }
            else if (left.Type == Symbol.ValueType.DOUBLE)
            {
                result.Value.bValue = left.Value.dValue == right.Value.dValue;
            }
            else if (left.Type == Symbol.ValueType.BOOL)
            {
                result.Value.bValue = left.Value.bValue == right.Value.bValue;
            }
            return result;
        }
    }

    public class NeqOp : BinOp
    {
        public override VarSymbol Calculate(VarSymbol left, VarSymbol right)
        {
            VarSymbol result = new VarSymbol();
            result.Type = Symbol.ValueType.BOOL;
            if (left.Type == Symbol.ValueType.INT)
            {
                result.Value.bValue = left.Value.iValue != right.Value.iValue;
            }
            else if (left.Type == Symbol.ValueType.DOUBLE)
            {
                result.Value.bValue = left.Value.dValue != right.Value.dValue;
            }
            else if (left.Type == Symbol.ValueType.BOOL)
            {
                result.Value.bValue = left.Value.bValue != right.Value.bValue;
            }
            return result;
        }
    }
}
