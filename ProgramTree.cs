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

        public Arguments Args { get; set; }

        public FunHeader(string type, string name, Arguments args)
        {
            if (args == null)
            {
                args = new Arguments();
            }
            Name = name;
            Symbol t = ParserHelper.GlobalTable.Get(type);
            if (!(t is TypeSymbol))
            {
                throw new SemanticExepction("Недопустимый тип аргумента " + Type + Name);
            }
            Type = (t as TypeSymbol).Value;
            Args = args;
        }
    }
   
    public class Arguments
    {
        public class Argument
        {
            public Symbol.ValueType Type { get; set; }

            public string Name { get; set; }

            public Argument(string type, string name)
            {
                Symbol t = ParserHelper.GlobalTable.Get(type);
                if (!(t is TypeSymbol) || (t as TypeSymbol).Value == Symbol.ValueType.VOID)
                {
                    throw new SemanticExepction("Недопустимый тип аргумента " + Type + Name);
                }
                Type = (t as TypeSymbol).Value;
                Name = name;
            }
        }

        public List<Argument> ArgList = new List<Argument>();

        public Arguments(string type, string name)
        {
            ArgList.Add(new Argument(type, name));
        }

        public Arguments()
        {

        }

        public void Add(string type, string name)
        {
            ArgList.Add(new Argument(type, name));
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
            ParserHelper.Stack.Push(new SymbolsRecord());
            VarSymbol result = FunList[FunList.Count - 1].Eval();
            ParserHelper.Stack.Pop();
            return result;
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
            Body.Exec();
            VarSymbol result = ParserHelper.BottomTable().Get(SymbolTable.RESULT) as VarSymbol;
            if (result.Type != Header.Type)
            {
                throw new SemanticExepction("Несоответствие типов кароч");
            }
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
                    leftValue = ParserHelper.upCast(leftValue, Symbol.ValueType.DOUBLE);
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
            // TODO: Сделать по-человечески (пожалуйста)
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

    public abstract class FStateStatementNode : StatementNode
    {

        private FinalState fState = FinalState.COMPLETE;

        public FinalState FState
        {
            get
            {
                return fState;
            }

            set
            {
                fState = value;
            }
        }

        public enum FinalState
        {
            RETURN,
            COMPLETE
        }
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
            VarSymbol leftValue = ParserHelper.TopTable().Get(Id.Name) as VarSymbol;
            VarSymbol exprVal = Expr.Eval();
            if (leftValue.Type != exprVal.Type)
            {
                throw new SemanticExepction("Incompatible assign types: " + 
                    leftValue.Type + " and " + exprVal.Type);
            }
            leftValue.Value = exprVal.Value;
            Console.WriteLine("{0} := int: {1}, double: {2}, bool: {3}",
                Id.Name, leftValue.Value.iValue, leftValue.Value.dValue, leftValue.Value.bValue);
        }
    }

    public class CondNode : FStateStatementNode
    {
        ExprNode Expr { get; set; }

        StatementNode StatIf { get; set; }

        StatementNode StatElse { get; set; }

        public CondNode(ExprNode expr, StatementNode statIf, StatementNode statElse)
        {
            Expr = expr;
            StatIf = statIf;
            StatElse = statElse;
        }

        public override void Exec()
        {
            ParserHelper.Stack.Peek().SavedTable = ParserHelper.TopTable();
            ParserHelper.Stack.Peek().TopTable = new SymbolTable(ParserHelper.TopTable());
            VarSymbol expr = Expr.Eval();
            if (expr.Type != Symbol.ValueType.BOOL)
            {
                throw new SemanticExepction("Несоответствие типов в выражении для оператора If");
            }
            if (expr.Value.bValue)
            {
                StatIf.Exec();
                if (StatIf is FStateStatementNode &&
                    (StatIf as FStateStatementNode).FState == FinalState.RETURN)
                {
                    FState = FinalState.RETURN;
                }
            }
            else
            {
                if (StatElse != null)
                {
                    StatElse.Exec();
                    if (StatElse is FStateStatementNode &&
                        (StatElse as FStateStatementNode).FState == FinalState.RETURN)
                    {
                        FState = FinalState.RETURN;
                    }
                }
            }
            ParserHelper.Stack.Peek().TopTable = ParserHelper.SavedTable();
        }
    }

    public class CycleNode : FStateStatementNode
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
            ParserHelper.Stack.Peek().SavedTable = ParserHelper.TopTable();
            ParserHelper.Stack.Peek().TopTable = new SymbolTable(ParserHelper.TopTable());
            VarSymbol val = Expr.Eval();
            if (val.Type != Symbol.ValueType.INT)
            {
                throw new SemanticExepction("Неверный тип в выражении для 'cycle'");
            }
            for (int i = 0; i < val.Value.iValue; ++i)
            {
                Stat.Exec();
                if (Stat is FStateStatementNode &&
                    (Stat as FStateStatementNode).FState == FinalState.RETURN)
                {
                    FState = FinalState.RETURN;
                    break;
                }
            }
            ParserHelper.Stack.Peek().TopTable = ParserHelper.SavedTable();
        }
    }

    public class BlockNode : FStateStatementNode
    {
        public List<StatementNode> StList = new List<StatementNode>();

        public BlockNode(StatementNode stat)
        {
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
                if ( stNode is FStateStatementNode &&
                    (stNode as FStateStatementNode).FState == FinalState.RETURN)
                {
                    FState = FinalState.RETURN;
                    break;
                }
            }
            ParserHelper.Stack.Peek().TopTable = ParserHelper.SavedTable();
        }

    }

    public class ProcCallNode : StatementNode
    {
        private FunCallNode FunCall { get; set; }

        public ProcCallNode(FunCallNode funCall)
        {
            FunCall = funCall;
        }

        public override void Exec()
        {
            FunCall.Eval();
        }
    }

    public class FunCallNode : ExprNode
    {
        public FunCallNode(string name, List<ExprNode> exprList)
        {
            Name = name;
            ExprList = exprList;
        }

        public FunCallNode(string name)
        {
            Name = name;
            ExprList = new List<ExprNode>();
        }

        public List<ExprNode> ExprList { get; set; }

        public string Name { get; set; }

        public override VarSymbol Eval()
        {
            FunSymbol fun = ParserHelper.GlobalTable.Get(Name) as FunSymbol;
            Arguments args = fun.Address.Header.Args;
            if (args.ArgList.Count != ExprList.Count)
            {
                throw new SemanticExepction("Неверное количество параметров при вызове функции " + Name);
            }
            List<VarSymbol> callArgs = new List<VarSymbol>();
            foreach (ExprNode expr in ExprList)
            {
                callArgs.Add(expr.Eval());
            }
            ParserHelper.Stack.Push(new SymbolsRecord());
            for (int i = 0; i < args.ArgList.Count; ++i)
            {
                if (callArgs[i].Type != args.ArgList[i].Type)
                {
                    throw new SemanticExepction("Несоответствие типов в параметре " + args.ArgList[i].Name + " функции " + Name);
                }
                ParserHelper.TopTable().Put(args.ArgList[i].Name, callArgs[i]);
            }
            VarSymbol Value = fun.Address.Eval();
            ParserHelper.Stack.Pop();
            return Value;
        }
    }

    public class DeclNode : StatementNode {

        public string Name { set; get; }

        public string Type { set; get; }

        public AssignNode Assign { get; set; }

        public DeclNode(string type, string name) {
            voidCheck(type);
            Name = name;
            Type = type;
        }

        public DeclNode(string type, AssignNode assign) {
            voidCheck(type);
            Type = type;
            Name = assign.Id.Name;
            Assign = assign;
        }

        private void voidCheck(string type) {
            if (type == "void") {
                throw new SemanticExepction("\'void\' cannot be used in this context.");
            }
        }

        public override void Exec() {
            VarSymbol s = new VarSymbol();
            Symbol t = (ParserHelper.GlobalTable.Get(Type));
            if (!(t is TypeSymbol)) {
                throw new SemanticExepction("Error type: " + Type);
            }
            s.Type = (t as TypeSymbol).Value;
            ParserHelper.TopTable().Put(Name, s);
            if (Assign != null) {
                Assign.Exec();
            }
        }
    }

    public class ReturnNode : FStateStatementNode
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
            FState = FinalState.RETURN;
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

    public class WhileNode : FStateStatementNode
    {

        public ExprNode Expr { get; set; }

        public StatementNode Stat { get; set; }

        public WhileNode(ExprNode expr, StatementNode stat)
        {
            Expr = expr;
            Stat = stat;
        }

        public override void Exec()
        {
            ParserHelper.Stack.Peek().SavedTable = ParserHelper.TopTable();
            ParserHelper.Stack.Peek().TopTable = new SymbolTable(ParserHelper.TopTable());
            VarSymbol expr = Expr.Eval();
            if (expr.Type != Symbol.ValueType.BOOL)
            {
                throw new SemanticExepction("Несоответствие типов в выражении для цикла while");
            }
            while (expr.Value.bValue)
            {
                Stat.Exec();
                if (Stat is FStateStatementNode &&
                    (Stat as FStateStatementNode).FState == FinalState.RETURN)
                {
                    FState = FinalState.RETURN;
                    break;
                }
                expr = Expr.Eval();
            }
            ParserHelper.Stack.Peek().TopTable = ParserHelper.SavedTable();
        }
    }

    public class DoWhileNode : FStateStatementNode
    {

        public ExprNode Expr { get; set; }

        public StatementNode Stat { get; set; }

        public DoWhileNode(ExprNode expr, StatementNode stat)
        {
            Expr = expr;
            Stat = stat;
        }

        public override void Exec()
        {
            ParserHelper.Stack.Peek().SavedTable = ParserHelper.TopTable();
            ParserHelper.Stack.Peek().TopTable = new SymbolTable(ParserHelper.TopTable());
            VarSymbol expr;
            do
            {
                Stat.Exec();
                if (Stat is FStateStatementNode &&
                    (Stat as FStateStatementNode).FState == FinalState.RETURN)
                {
                    FState = FinalState.RETURN;
                    break;
                }
                expr = Expr.Eval();
                if (expr.Type != Symbol.ValueType.BOOL)
                {
                    throw new SemanticExepction("Несоответствие типов в выражении для цикла do while");
                }
            } while (expr.Value.bValue);
            ParserHelper.Stack.Peek().TopTable = ParserHelper.SavedTable();
        }
    }

    public class ForNode: FStateStatementNode
    {
        public StatementNode Init { get; set; }

        public StatementNode Iter { get; set; }

        public ExprNode Cond { get; set; }

        public StatementNode Stat { get; set; }

        public ForNode(StatementNode init, ExprNode cond, StatementNode iter, StatementNode stat)
        {
            if (!(init is DeclNode || init is AssignNode))
            {
                throw new SemanticExepction("Invalid initialization section in \'for\' cycle");
            }
            Init = init;
            Cond = cond;
            if (!(iter is ProcCallNode || iter is AssignNode))
            {
                throw new SemanticExepction("Invalid iteration section in \'for\' cycle");
            }
            Iter = iter;
            Stat = stat;
        }

        public override void Exec()
        {
            ParserHelper.Stack.Peek().SavedTable = ParserHelper.TopTable();
            ParserHelper.Stack.Peek().TopTable = new SymbolTable(ParserHelper.TopTable());
            Init.Exec();
            VarSymbol init_var = null;
            if (Init is DeclNode)
            {
                init_var = ParserHelper.TopTable().Get((Init as DeclNode).Name) as VarSymbol;
                 
            } else
            {
                init_var = ParserHelper.TopTable().Get((Init as AssignNode).Id.Name) as VarSymbol;
            }
            VarSymbol cond_val = Cond.Eval();
            if (cond_val.Type != Symbol.ValueType.BOOL)
            {
                throw new SemanticExepction("Invalid condition expression in \'for\' cycle");
            }
            while (cond_val.Value.bValue)
            {
                bool isBlock = Stat is BlockNode;
                if (!isBlock)
                {
                    ParserHelper.Stack.Peek().SavedTable = ParserHelper.TopTable();
                    ParserHelper.Stack.Peek().TopTable = new SymbolTable(ParserHelper.TopTable());
                }
                Stat.Exec();
                if (Stat is FStateStatementNode &&
                    (Stat as FStateStatementNode).FState == FinalState.RETURN)
                {
                    FState = FinalState.RETURN;
                    if (!isBlock)
                    {
                        ParserHelper.Stack.Peek().TopTable = ParserHelper.SavedTable();
                    }
                    break;
                }
                Iter.Exec();
                cond_val = Cond.Eval();
                if (!isBlock)
                {
                    ParserHelper.Stack.Peek().TopTable = ParserHelper.SavedTable();
                }
            }
            ParserHelper.Stack.Peek().TopTable = ParserHelper.SavedTable();
        }
    }
}