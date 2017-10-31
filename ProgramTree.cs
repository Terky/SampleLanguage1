using System;
using System.Collections.Generic;
using SimpleLang;
using SimpleParser;

namespace ProgramTree
{
    public enum AssignType { Assign, AssignPlus, AssignMinus, AssignMult, AssignDivide };

    public enum OpType { Plus, Minus, Div, Mult };

    public class Node // базовый класс для всех узлов    
    {
    }

    public class FunListNode : Node
    {
        public List<FunNode> FunList = new List<FunNode>();

        public FunListNode(FunNode fun)
        {
            Add(fun);
        }

        public void Add(FunNode fun)
        {
            FunList.Add(fun);
        }
    }

    public class FunNode : ExprNode
    {
        //
        public FunNode(string name, string type, BlockNode body)
        {
            Name = name;
            Type = type;
            Body = body;
        }
        public Symbol.ValueType Type { get; set; }

        public string Name { get; set; }

        public BlockNode Body { get; set; }

        public override VarSymbol Eval()
        {
            Body.Exec();
            
        }
    }

    public abstract class ExprNode : Node // базовый класс для всех выражений
    {
        public abstract VarSymbol Eval();
    }

    public class IdNode : ExprNode
    {
        public IdNode(string name) {
            Name = name;
        }

        public string Name { get; set; }

        public VarSymbol Value { get; set; }

        public override VarSymbol Eval()
        {
            VarSymbol s = ParserHelper.top.Get(Name) as VarSymbol;
            if (s == null)
            {
                //error
            }
            Value = s;
            return Value;
        }
    }

    public class BinExprNode : ExprNode
    {
        public BinExprNode(ExprNode left, ExprNode right, OpType op)
        {
            Left = left;
            Right = right;
            Op = op;
        }

        public ExprNode Left { get; set; }

        public ExprNode Right { get; set; }

        public OpType Op { get; set; }

        public override VarSymbol Eval()
        {
            VarSymbol leftValue = Left.Eval();
            VarSymbol rightValue = Right.Eval();
            if (leftValue.Type != rightValue.Type)
            {
                if (leftValue.Type == Symbol.ValueType.INT && rightValue.Type == Symbol.ValueType.DOUBLE)
                {
                    ParserHelper.upCast(leftValue, Symbol.ValueType.DOUBLE);
                } else if (leftValue.Type == Symbol.ValueType.DOUBLE && rightValue.Type == Symbol.ValueType.INT)
                {
                    ParserHelper.upCast(rightValue, Symbol.ValueType.DOUBLE);
                } else
                {
                    //error
                }
            }

            VarSymbol res = new VarSymbol();
            switch (Op)
            {
                case OpType.Plus:
                    switch (leftValue.Type)
                    {
                        case Symbol.ValueType.DOUBLE:
                            res.Type = Symbol.ValueType.DOUBLE;
                            res.Value.dValue = leftValue.Value.dValue + rightValue.Value.dValue;
                            break;
                        case Symbol.ValueType.INT:
                            res.Type = Symbol.ValueType.INT;
                            res.Value.iValue = leftValue.Value.iValue + rightValue.Value.iValue;
                            break;
                    }
                    break;
                case OpType.Minus:
                    switch (leftValue.Type)
                    {
                        case Symbol.ValueType.DOUBLE:
                            res.Type = Symbol.ValueType.DOUBLE;
                            res.Value.dValue = leftValue.Value.dValue - rightValue.Value.dValue;
                            break;
                        case Symbol.ValueType.INT:
                            res.Type = Symbol.ValueType.INT;
                            res.Value.iValue = leftValue.Value.iValue - rightValue.Value.iValue;
                            break;
                    }
                    break;
                case OpType.Mult:
                    switch (leftValue.Type)
                    {
                        case Symbol.ValueType.DOUBLE:
                            res.Type = Symbol.ValueType.DOUBLE;
                            res.Value.dValue = leftValue.Value.dValue * rightValue.Value.dValue;
                            break;
                        case Symbol.ValueType.INT:
                            res.Type = Symbol.ValueType.INT;
                            res.Value.iValue = leftValue.Value.iValue * rightValue.Value.iValue;
                            break;
                    }
                    break;
                case OpType.Div:
                    switch (leftValue.Type)
                    {
                        case Symbol.ValueType.DOUBLE:
                            res.Type = Symbol.ValueType.DOUBLE;
                            res.Value.dValue = leftValue.Value.dValue / rightValue.Value.dValue;
                            break;
                        case Symbol.ValueType.INT:
                            res.Type = Symbol.ValueType.INT;
                            res.Value.iValue = leftValue.Value.iValue / rightValue.Value.iValue;
                            break;
                    }
                    break;
            }
            return res;
        }

    }

    public class IntNumNode : ExprNode
    {
        public int Num { get; set; }
        public IntNumNode(int num) { Num = num; }
        public override VarSymbol Eval()
        {
            VarSymbol value = new VarSymbol();
            value.Type = Symbol.ValueType.INT;
            value.Value.iValue = Num;
            return value;
        }
    }

    public class DoubleNumNode : ExprNode
    {
        public double Num { get; set; }
        public DoubleNumNode(double num) { Num = num; }
        public override VarSymbol Eval()
        {
            VarSymbol value = new VarSymbol();
            value.Type = Symbol.ValueType.DOUBLE;
            value.Value.dValue = Num;
            return value;
        }
    }

    public class BoolNode : ExprNode
    {
        public BoolNode(bool val) { Val = val; }
        public bool Val { get; set;}
        public override VarSymbol Eval()
        {
            VarSymbol value = new VarSymbol();
            value.Type = Symbol.ValueType.BOOL;
            value.Value.bValue = Val;
            return value;
        }
    }

    public abstract class StatementNode : Node // базовый класс для всех операторов
    {
        public abstract void Exec();
    }

    public class AssignNode : StatementNode
    {
        public IdNode Id { get; set; }
        public ExprNode Expr { get; set; }
        public AssignType AssOp { get; set; }
        public AssignNode(IdNode id, ExprNode expr, AssignType assop = AssignType.Assign)
        {
            Id = id;
            Expr = expr;
            AssOp = assop;
        }
        public override void Exec()
        {
            VarSymbol s = ParserHelper.top.Get(Id.Name) as VarSymbol;
            if (s == null)
            { // Возможно недостижимый код
                throw new SemanticExepction("How do you get here?");
            }
            VarSymbol exprVal = Expr.Eval();
            if (s.Type != exprVal.Type)
            {
                //error - несовместимые типы. Нужно сделать совместимыми не только равные типы
            }
			s.Value = exprVal.Value;
            Console.WriteLine("{0} := int: {1}, double: {2}, bool: {3}", Id.Name, s.Value.iValue, s.Value.dValue, s.Value.bValue);
        }
    }

    public class CycleNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public CycleNode(ExprNode expr, StatementNode stat)
        {
            Expr = expr;
            Stat = stat;
        }
        public override void Exec()
        {
            VarSymbol val = Expr.Eval();
            if (val.Type != Symbol.ValueType.INT)
            {
                //error
            }
            for (int i = 0; i < val.Value.iValue; ++i)
            {
                Stat.Exec();
            }
        }
    }

    public class BlockNode : StatementNode
    {
        public List<StatementNode> StList = new List<StatementNode>();

        public BlockNode(StatementNode stat)
        {
            Add(stat);
        }

        public void Add(StatementNode stat)
        {
            StList.Add(stat);
        }

        public override void Exec()
        {
            ParserHelper.saved = ParserHelper.top;
            ParserHelper.top = new SymbolTable(ParserHelper.top);
            foreach (StatementNode stNode in StList)
            {
                stNode.Exec();
            }
            ParserHelper.top = ParserHelper.saved;
        }

    }

    public class DeclNode : StatementNode
    {

        public string Name { set; get; }

        public string Type { set; get; }

        public DeclNode(string name, string type)
        {
            if (type == "void")
            {
                throw new SemanticExepction("\'void\' cannot be used in this context.");
            }
            Name = name;
            Type = type;
        }

        public override void Exec()
        {
            VarSymbol s = new VarSymbol();
            TypeSymbol t = (ParserHelper.top.Get(Type)) as TypeSymbol;
            s.Type = t.Value;
            ParserHelper.top.Put(Name, s);
        }
    }
}