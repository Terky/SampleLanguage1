using System;
using System.Collections.Generic;
using SimpleLang;
using SimpleParser;
using QUT.Gppg;

namespace ProgramTree
{
    public enum AssignType { Assign, AssignPlus, AssignMinus, AssignMult, AssignDivide };

    public enum OpType { Plus, Minus, Div, Mult, Or, And, Not, Lt, Gt, Let, Get, Eq, Neq };

    public class FunHeader
    {
        public Symbol.ValueType Type { get; set; }

        public string Name { get; set; }

        public FormalParams Args { get; set; }

        public FunHeader(string type, string name, FormalParams args)
        {
            if (args == null)
            {
                args = new FormalParams();
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
   
    public class FormalParams
    {
        public class FormalParam
        {
            public Symbol.ValueType Type { get; set; }

            public DeclId Name { get; set; }

            public FormalParam(DeclType type, DeclId name)
            {
                Type = type.Type;
                Name = name;
            }
        }

        public List<FormalParam> FormalParamList = new List<FormalParam>();

        public FormalParams(DeclType type, DeclId name)
        {
            FormalParamList.Add(new FormalParam(type, name));
        }

        public FormalParams()
        {

        }

        public void Add(DeclType type, DeclId name)
        {
            FormalParamList.Add(new FormalParam(type, name));
        }
    }

    public class DeclType
    {
        public Symbol.ValueType Type { get; set; }

        public DeclType(string type, LexLocation loc)
        {
            Symbol t = ParserHelper.GlobalTable.Get(type);
            if (!(t is TypeSymbol) || (t as TypeSymbol).Value == Symbol.ValueType.VOID)
            {
                throw new SemanticExepction("Недопустимый тип аргумента " + Type);
            }
            Type = (t as TypeSymbol).Value;
        }
    }

    public class DeclId
    {
        public string Name { get; set; }

        public DeclId(string name, LexLocation loc)
        {
            Name = name;
        }
    }

    public abstract class Node // базовый класс для всех узлов    
    {
        public abstract void Visit(Visitor v);
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

        public override void Visit(Visitor v)
        {
            v.Visit(this);
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

        public override void Visit(Visitor v)
        {
            v.Visit(this);
        }
    }

    public abstract class ExprNode : Node // базовый класс для всех выражений
    {
    }

    public class IdNode : ExprNode
    {
        public IdNode(string name) {
            Name = name;
        }

        public string Name { get; set; }

        public override void Visit(Visitor v)
        {
            v.Visit(this);
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

        public override void Visit(Visitor v)
        {
            v.Visit(this);
        }

    }

    public class UnExprNode : ExprNode
    {
        public ExprNode Expr { get; set; }

        public OpType Op { get; set; }

        public UnExprNode(ExprNode expr, OpType op)
        {
            Expr = expr;
            Op = op;
        }

        public override void Visit(Visitor v)
        {
            v.Visit(this);
        }
    }

    public class IntNumNode : ExprNode
    {
        public int Num { get; set; }

        public IntNumNode(int num) { Num = num; }

        public override void Visit(Visitor v)
        {
            v.Visit(this);
        }
    }

    public class DoubleNumNode : ExprNode
    {
        public double Num { get; set; }

        public DoubleNumNode(double num) { Num = num; }

        public override void Visit(Visitor v)
        {
            v.Visit(this);
        }
    }

    public class BoolNode : ExprNode
    {
        public BoolNode(bool val) { Val = val; }

        public bool Val { get; set;}

        public override void Visit(Visitor v)
        {
            v.Visit(this);
        }
    }

    public abstract class StatementNode : Node // базовый класс для всех операторов
    {
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

        public override void Visit(Visitor v)
        {
            v.Visit(this);
        }
    }

    public class CondNode : StatementNode
    {
        public ExprNode Expr { get; set; }

        public StatementNode StatIf { get; set; }

        public StatementNode StatElse { get; set; }

        public CondNode(ExprNode expr, StatementNode statIf, StatementNode statElse)
        {
            Expr = expr;
            StatIf = statIf;
            StatElse = statElse;
        }

        public override void Visit(Visitor v)
        {
            v.Visit(this);
        }
    }

    public class BlockNode : StatementNode
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

        public override void Visit(Visitor v)
        {
            v.Visit(this);
        }

    }

    public class ProcCallNode : StatementNode
    {
        public FunCallNode FunCall { get; set; }

        public ProcCallNode(FunCallNode funCall)
        {
            FunCall = funCall;
        }

        public override void Visit(Visitor v)
        {
            v.Visit(this);
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

        public override void Visit(Visitor v)
        {
            v.Visit(this);
        }
    }

    public class DeclNode : StatementNode {

        public string Name { set; get; }

        public Symbol.ValueType Type { set; get; }

        public AssignNode Assign { get; set; }

        public DeclNode(string type, string name) {
            InitType(type);
            Name = name;
        }

        public DeclNode(string type, AssignNode assign) {
            InitType(type);
            Name = assign.Id.Name;
            Assign = assign;
        }

        private void InitType(string type)
        {
            Symbol sym = ParserHelper.GlobalTable.Get(type);
            if (!(sym is TypeSymbol))
            {
                throw new SemanticExepction("Error type: " + Type);
            }
            Type = (sym as TypeSymbol).Value;
        }

        public override void Visit(Visitor v)
        {
            v.Visit(this);
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

        public override void Visit(Visitor v)
        {
            v.Visit(this);
        }
    }

    public class WhileNode : StatementNode
    {

        public ExprNode Expr { get; set; }

        public StatementNode Stat { get; set; }

        public WhileNode(ExprNode expr, StatementNode stat)
        {
            Expr = expr;
            Stat = stat;
        }

        public override void Visit(Visitor v)
        {
            v.Visit(this);
        }
    }

    public class DoWhileNode : StatementNode
    {

        public ExprNode Expr { get; set; }

        public StatementNode Stat { get; set; }

        public DoWhileNode(ExprNode expr, StatementNode stat)
        {
            Expr = expr;
            Stat = stat;
        }

        public override void Visit(Visitor v)
        {
            v.Visit(this);
        }
    }

    public class ForNode: StatementNode
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

        public override void Visit(Visitor v)
        {
            v.Visit(this);
        }
    }
}