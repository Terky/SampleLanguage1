using System;
using System.Collections.Generic;
using SimpleLang;
using SimpleParser;
using QUT.Gppg;

namespace ProgramTree
{
    public enum AssignType { Assign, AssignPlus, AssignMinus, AssignMult, AssignDivide };

    public enum OpType { Plus, Minus, Div, Mult, Or, And, Not, Lt, Gt, Let, Get, Eq, Neq };

    public class DeclList
    {
        public class Decl
        {
            public AssignNode Assign { get; set; }

            public DeclId Name { get; set; }

            public Decl(DeclId name, DeclAssign assign)
            {
                Assign =
                    assign == null ?
                        null :
                        new AssignNode(new IdNode(name.Name, name.LexLoc), assign.Expr);
                Name = name;
            }
        }

        public List<Decl> DeclsList { get; set; }

        public DeclList(DeclId name, DeclAssign assign)
        {
            DeclsList = new List<Decl>();
            DeclsList.Add(new Decl(name, assign));
        }

        public void Add(DeclId name, DeclAssign assign)
        {
            DeclsList.Add(new Decl(name, assign));
        }
}

    public class FunHeader
    {
        public Symbol.ValueType Type { get; set; }

        public string Name { get; set; }

        public FormalParams Args { get; set; }

        public FunHeader(DeclType type, DeclId name, FormalParams args)
        {
            if (args == null)
            {
                args = new FormalParams();
            }
            Name = name.Name;
            Type = type.Type;
            Args = args;
        }
    }

    public class DeclAssign
    {
        public ExprNode Expr { get; set; }

        public DeclAssign(ExprNode expr)
        {
            Expr = expr;
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
        public LexLocation LexLoc { get; set; }

        public Symbol.ValueType Type { get; set; }

        public DeclType(string type, LexLocation lexLoc)
        {
            LexLoc = lexLoc;
            Symbol t = ParserHelper.GlobalTable.Get(type);
            if (!(t is TypeSymbol))
            {
                throw new SemanticExepction("Недопустимый тип аргумента " + Type + 
                    ". Строка " + lexLoc.StartLine + ", столбец " + lexLoc.StartColumn);
            }
            Type = (t as TypeSymbol).Value;
        }
    }

    public class DeclId
    {

        public LexLocation LexLoc { get; set; }

        public string Name { get; set; }

        public DeclId(string name, LexLocation lexLoc)
        {
            LexLoc = lexLoc;
            Name = name;
        }
    }

    public abstract class Node // базовый класс для всех узлов    
    {
        public abstract T Visit<T>(Visitor<T> v);
    }

    public class MainProgramNode : ExprNode
    {
        public List<FunNode> FunList = new List<FunNode>();

        public MainProgramNode(FunNode fun, LexLocation lexLoc) : base(lexLoc)
        {
            Add(fun);
        }

        public void Add(FunNode fun)
        {
            FunList.Add(fun);
        }

        public override T Visit<T>(Visitor<T> v)
        {
            return v.Visit(this);
        }
    }

    public class FunNode : ExprNode
    {
        public FunHeader Header { get; set; }

        public BlockNode Body { get; set; }

        public FunNode(FunHeader header, BlockNode body, LexLocation lexLoc) : base(lexLoc)
        {
            Header = header;
            Body = body;
            FunSymbol funSymbol = new FunSymbol(header.Type, this);
            ParserHelper.GlobalTable.Put(Header.Name, funSymbol);
        }

        public override T Visit<T>(Visitor<T> v)
        {
            return v.Visit(this);
        }
    }

    public abstract class ExprNode : Node // базовый класс для всех выражений
    {
        public LexLocation LexLoc { get; set; }

        public ExprNode(LexLocation lexLoc)
        {
            LexLoc = lexLoc;
        }
    }

    public class IdNode : ExprNode
    {
        public IdNode(string name, LexLocation lexLoc): base(lexLoc)
        {
            Name = name;
        }

        public string Name { get; set; }

        public override T Visit<T>(Visitor<T> v)
        {
            return v.Visit(this);
        }
    }

    public class BinExprNode : ExprNode
    {
        public BinExprNode(ExprNode left, ExprNode right, OpType op, LexLocation lexLoc) : base(lexLoc)
        {
            Left = left;
            Right = right;
            Op = op;
        }

        public ExprNode Left { get; set; }

        public ExprNode Right { get; set; }

        public OpType Op { get; set; }

        public override T Visit<T>(Visitor<T> v)
        {
            return v.Visit(this);
        }

    }

    public class UnExprNode : ExprNode
    {
        public ExprNode Expr { get; set; }

        public OpType Op { get; set; }

        public UnExprNode(ExprNode expr, OpType op, LexLocation lexLoc) : base(lexLoc)
        {
            Expr = expr;
            Op = op;
        }

        public override T Visit<T>(Visitor<T> v)
        {
            return v.Visit(this);
        }
    }

    public class IntNumNode : ExprNode
    {
        public int Num { get; set; }

        public IntNumNode(int num, LexLocation lexLoc) : base(lexLoc) { Num = num; }

        public override T Visit<T>(Visitor<T> v)
        {
            return v.Visit(this);
        }
    }

    public class DoubleNumNode : ExprNode
    {
        public double Num { get; set; }

        public DoubleNumNode(double num, LexLocation lexLoc) : base(lexLoc) { Num = num; }

        public override T Visit<T>(Visitor<T> v)
        {
            return v.Visit(this);
        }
    }

    public class BoolNode : ExprNode
    {
        public BoolNode(bool val, LexLocation lexLoc) : base(lexLoc) { Val = val; }

        public bool Val { get; set;}

        public override T Visit<T>(Visitor<T> v)
        {
            return v.Visit(this);
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

        public override T Visit<T>(Visitor<T> v)
        {
            return v.Visit(this);
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

        public override T Visit<T>(Visitor<T> v)
        {
            return v.Visit(this);
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

        public override T Visit<T>(Visitor<T> v)
        {
            return v.Visit(this);
        }

    }

    public class ProcCallNode : StatementNode
    {
        public FunCallNode FunCall { get; set; }

        public ProcCallNode(FunCallNode funCall)
        {
            FunCall = funCall;
        }

        public override T Visit<T>(Visitor<T> v)
        {
            return v.Visit(this);
        }
    }

    public class FunCallNode : ExprNode
    {
        public FunCallNode(string name, List<ExprNode> actualParams, LexLocation lexLoc) : base(lexLoc)
        {
            Name = name;
            if (actualParams == null)
            {
                ActualParams = new List<ExprNode>();
            } else
            {
                ActualParams = actualParams;
            }
        }

        public List<ExprNode> ActualParams { get; set; }

        public string Name { get; set; }

        public override T Visit<T>(Visitor<T> v)
        {
            return v.Visit(this);
        }
    }

    public class DeclNode : StatementNode {
        
        public LexLocation LexLoc { get; set; }

        public Symbol.ValueType Type { set; get; }

        public DeclList DeclsList { get; set; }

        public DeclNode(DeclType type, DeclList declsList) {
            LexLoc = type.LexLoc;
            Type = type.Type;
            DeclsList = declsList;
        }

        public override T Visit<T>(Visitor<T> v)
        {
            return v.Visit(this);
        }
    }

    public class ReturnNode : StatementNode
    {
        public ExprNode Expr { get; set; }

        public ReturnNode(ExprNode expr)
        {
            Expr = expr;
        }

        public override T Visit<T>(Visitor<T> v)
        {
            return v.Visit(this);
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

        public override T Visit<T>(Visitor<T> v)
        {
            return v.Visit(this);
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

        public override T Visit<T>(Visitor<T> v)
        {
            return v.Visit(this);
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

        public override T Visit<T>(Visitor<T> v)
        {
            return v.Visit(this);
        }
    }
}