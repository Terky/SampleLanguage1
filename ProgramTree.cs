using System;
using System.Collections.Generic;
using SimpleLang;
using SimpleParser;

namespace ProgramTree
{
    public enum AssignType { Assign, AssignPlus, AssignMinus, AssignMult, AssignDivide };

    public enum OpType { Plus, Minus, Div, Mult, Or, And, Not, Lt, Gt, Let, Get, Eq, Neq };

    public class FunHeader
    {
        public Symbol.ValueType Type { get; set; }

        public string Name { get; set; }

        public FunHeader(string type, string name)
        {
            Name = name;
            Type = (ParserHelper.GlobalTable.Get(type) as TypeSymbol).Value;
        }

    }

    public class Node // базовый класс для всех узлов    
    {
    }

    public class MainProgramNode : ExprNode
    {
        public List<FunNode> FunList = new List<FunNode>();

        public MainProgramNode(FunNode fun)
        {
            Add(fun);
        }

        public void Add(FunNode fun)
        {
            FunList.Add(fun);
        }

        public override VarSymbol Eval()
        {
            return FunList[FunList.Count - 1].Eval();
        }
    }

    public class FunNode : ExprNode
    {
        public FunHeader Header { get; set; }

        public BlockNode Body { get; set; }

        public FunNode(FunHeader header, BlockNode body)
        {
            Header = header;
            Body = body;
            FunSymbol funSymbol = new FunSymbol(header.Type, this);
            ParserHelper.GlobalTable.Put(Header.Name, funSymbol);
        }

        public override VarSymbol Eval()
        {
            ParserHelper.Stack.Push(new SymbolsRecord());
            Body.Exec();
            VarSymbol result = ParserHelper.BottomTable().Get(SymbolTable.RESULT) as VarSymbol;
            if (result.Type != Header.Type)
            {
                throw new SemanticExepction("Несоответствие типов кароч");
            }
            //может быть ошибка
            ParserHelper.Stack.Pop();
            return result;
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
            VarSymbol s = ParserHelper.TopTable().Get(Name) as VarSymbol;
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
                    throw new SemanticExepction("Несоответствие типов, оператор " + Op.ToString());
                }
            }

            VarSymbol res = new VarSymbol();
            //-----------------------------------------
            //TODO: Сделать по-человечески (пожалуйста)
            //-----------------------------------------
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
                        default:
                            throw new SemanticExepction("Оператор " + Op.ToString() + " применяется к неподходящим типам");
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
                        default:
                            throw new SemanticExepction("Оператор " + Op.ToString() + " применяется к неподходящим типам");
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
                        default:
                            throw new SemanticExepction("Оператор " + Op.ToString() + " применяется к неподходящим типам");
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
                        default:
                            throw new SemanticExepction("Оператор " + Op.ToString() + " применяется к неподходящим типам");
                    }
                    break;
                case OpType.And:
                    switch (leftValue.Type)
                    {
                        case Symbol.ValueType.BOOL:
                            res.Type = Symbol.ValueType.BOOL;
                            res.Value.bValue = leftValue.Value.bValue && rightValue.Value.bValue;
                            break;
                        default:
                            throw new SemanticExepction("Оператор " + Op.ToString() + " применяется к неподходящим типам");
                    }
                    break;
                case OpType.Or:
                    switch (leftValue.Type)
                    {
                        case Symbol.ValueType.BOOL:
                            res.Type = Symbol.ValueType.BOOL;
                            res.Value.bValue = leftValue.Value.bValue || rightValue.Value.bValue;
                            break;
                        default:
                            throw new SemanticExepction("Оператор " + Op.ToString() + " применяется к неподходящим типам");
                    }
                    break;
                case OpType.Gt:
                    switch (leftValue.Type)
                    {
                        case Symbol.ValueType.INT:
                            res.Type = Symbol.ValueType.BOOL;
                            res.Value.bValue = leftValue.Value.iValue > rightValue.Value.iValue;
                            break;
                        case Symbol.ValueType.DOUBLE:
                            res.Type = Symbol.ValueType.BOOL;
                            res.Value.bValue = leftValue.Value.dValue > rightValue.Value.dValue;
                            break;
                        default:
                            throw new SemanticExepction("Оператор " + Op.ToString() + " применяется к неподходящим типам");
                    }
                    break;
                case OpType.Lt:
                    switch (leftValue.Type)
                    {
                        case Symbol.ValueType.INT:
                            res.Type = Symbol.ValueType.BOOL;
                            res.Value.bValue = leftValue.Value.iValue < rightValue.Value.iValue;
                            break;
                        case Symbol.ValueType.DOUBLE:
                            res.Type = Symbol.ValueType.BOOL;
                            res.Value.bValue = leftValue.Value.dValue < rightValue.Value.dValue;
                            break;
                        default:
                            throw new SemanticExepction("Оператор " + Op.ToString() + " применяется к неподходящим типам");
                    }
                    break;
                case OpType.Get:
                    switch (leftValue.Type)
                    {
                        case Symbol.ValueType.INT:
                            res.Type = Symbol.ValueType.BOOL;
                            res.Value.bValue = leftValue.Value.iValue >= rightValue.Value.iValue;
                            break;
                        case Symbol.ValueType.DOUBLE:
                            res.Type = Symbol.ValueType.BOOL;
                            res.Value.bValue = leftValue.Value.dValue >= rightValue.Value.dValue;
                            break;
                        default:
                            throw new SemanticExepction("Оператор " + Op.ToString() + " применяется к неподходящим типам");
                    }
                    break;
                case OpType.Let:
                    switch (leftValue.Type)
                    {
                        case Symbol.ValueType.INT:
                            res.Type = Symbol.ValueType.BOOL;
                            res.Value.bValue = leftValue.Value.iValue <= rightValue.Value.iValue;
                            break;
                        case Symbol.ValueType.DOUBLE:
                            res.Type = Symbol.ValueType.BOOL;
                            res.Value.bValue = leftValue.Value.dValue <= rightValue.Value.dValue;
                            break;
                        default:
                            throw new SemanticExepction("Оператор " + Op.ToString() + " применяется к неподходящим типам");
                    }
                    break;
                case OpType.Eq:
                    switch (leftValue.Type)
                    {
                        case Symbol.ValueType.INT:
                            res.Type = Symbol.ValueType.BOOL;
                            res.Value.bValue = leftValue.Value.iValue == rightValue.Value.iValue;
                            break;
                        case Symbol.ValueType.DOUBLE:
                            res.Type = Symbol.ValueType.BOOL;
                            res.Value.bValue = leftValue.Value.dValue == rightValue.Value.dValue;
                            break;
                        default:
                            throw new SemanticExepction("Оператор " + Op.ToString() + " применяется к неподходящим типам");
                    }
                    break;
                case OpType.Neq:
                    switch (leftValue.Type)
                    {
                        case Symbol.ValueType.INT:
                            res.Type = Symbol.ValueType.BOOL;
                            res.Value.bValue = leftValue.Value.iValue != rightValue.Value.iValue;
                            break;
                        case Symbol.ValueType.DOUBLE:
                            res.Type = Symbol.ValueType.BOOL;
                            res.Value.bValue = leftValue.Value.dValue != rightValue.Value.dValue;
                            break;
                        default:
                            throw new SemanticExepction("Оператор " + Op.ToString() + " применяется к неподходящим типам");
                    }
                    break;
            }
            return res;
        }

    }

    public class UnExprNode : ExprNode
    {
        ExprNode Expr { get; set; }

        OpType Op { get; set; }

        public UnExprNode(ExprNode expr, OpType op)
        {
            Expr = expr;
            Op = op;
        }

        public override VarSymbol Eval()
        {
            switch (Op)
            {
                case OpType.Not:
                    VarSymbol res = Expr.Eval();
                    if (res.Type != Symbol.ValueType.BOOL)
                    {
                        throw new SemanticExepction("Несоответствие типов, оператор '!'");
                    }
                    VarSymbol.VarValue value = new VarSymbol.VarValue();
                    value.bValue = !res.Value.bValue;
                    return new VarSymbol(Symbol.ValueType.BOOL, value);
                default:
                    throw new SemanticExepction("Недопустимый унарный оператор");
            }
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
            VarSymbol s = ParserHelper.TopTable().Get(Id.Name) as VarSymbol;
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
        public enum FinalState
        {
            RETURN,
            COMPLETE
        }

        public FinalState FState { get; set; }

        public List<StatementNode> StList = new List<StatementNode>();

        public BlockNode(StatementNode stat)
        {
            FState = FinalState.COMPLETE;
            if (stat != null)
            {
                Add(stat);
            }
        }

        public void Add(StatementNode stat)
        {
            StList.Add(stat);
        }

        public override void Exec()
        {
            ParserHelper.Stack.Peek().SavedTable = ParserHelper.TopTable();
            ParserHelper.Stack.Peek().TopTable = new SymbolTable(ParserHelper.TopTable());
            foreach (StatementNode stNode in StList)
            {
                stNode.Exec();
                if (stNode is ReturnNode)
                {
                    FState = FinalState.RETURN;
                    break;
                }
                if ( stNode is BlockNode &&
                    (stNode as BlockNode).FState == FinalState.RETURN)
                {
                    FState = FinalState.RETURN;
                    break;
                }
            }
            ParserHelper.Stack.Peek().TopTable = ParserHelper.SavedTable();
        }

    }

    public class FunCallNode : ExprNode
    {
        public FunCallNode(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public override VarSymbol Eval()
        {
            FunSymbol fun = ParserHelper.GlobalTable.Get(Name) as FunSymbol;
            if (fun == null)
            {
                //error
            }
            VarSymbol Value = fun.Address.Eval();
            return Value;
        }
    }

    public class DeclNode : StatementNode
    {

        public string Name { set; get; }

        public string Type { set; get; }

        public DeclNode(string type, string name)
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
            TypeSymbol t = (ParserHelper.GlobalTable.Get(Type)) as TypeSymbol;
            s.Type = t.Value;
            ParserHelper.TopTable().Put(Name, s);
        }
    }

    public class ReturnNode : StatementNode
    {
        public ExprNode Expr { get; set; }

        public ReturnNode(ExprNode expr)
        {
            Expr = expr;
        }

        //Для void
        public ReturnNode() { }

        public override void Exec()
        {
            VarSymbol value;
            if (Expr != null) {
                value = Expr.Eval();
            } else
            {
                value = new VarSymbol();
                value.Type = Symbol.ValueType.VOID;
            }
            VarSymbol result = ParserHelper.BottomTable().Get(SymbolTable.RESULT) as VarSymbol;
            result.Type = value.Type;
            result.Value = value.Value;
        }
    }
}