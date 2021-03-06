﻿using System;
using System.Collections.Generic;
using SimpleLang;
using SimpleParser;
using QUT.Gppg;

namespace ProgramTree
{
    public enum AssignType { Assign, AssignPlus, AssignMinus, AssignMult, AssignDivide };

    public enum OpType { Plus, Minus, Div, Mult, Or, And, Not, Lt, Gt, Let, Get, Eq, Neq };

    public abstract class GrammarType { } // Базовый класс для грамматических типов

    public class DeclList: GrammarType
    {
        public class Decl
        {
            public AssignNode Assign { get; set; }

            public DeclId Id { get; set; }

            public Decl(DeclId name, DeclAssign assign, LexLocation lexLoc)
            {
                Assign =
                    assign == null ?
                        null :
                        new AssignNode(new IdNode(name.Name, name.LexLoc), assign.Expr, name.LexLoc);
                Id = name;
            }
        }

        public LexLocation LexLoc { get; set; }

        public List<Decl> Decls { get; set; }

        public DeclList(DeclId name, DeclAssign assign, LexLocation lexLoc)
        {
            LexLoc = lexLoc;
            Decls = new List<Decl>();
            Decls.Add(new Decl(name, assign, name.LexLoc));
        }

        public void Add(DeclId name, DeclAssign assign)
        {
            Decls.Add(new Decl(name, assign, name.LexLoc));
        }
}

    public class FunHeader: GrammarType
    {
        public LexLocation LexLoc { get; set; }

        public Symbol.ValueType Type { get; set; }

        public string Name { get; set; }

        public FormalParams Args { get; set; }

        public FunHeader(DeclType type, DeclId name, FormalParams args, LexLocation lexLoc)
        {
            LexLoc = lexLoc;
            if (args == null)
            {
                args = new FormalParams();
            }
            Name = name.Name;
            Type = type.Type;
            Args = args;
        }
    }

    public class DeclAssign: GrammarType
    {
        public LexLocation LexLoc { get; set; }

        public ExprNode Expr { get; set; }

        public DeclAssign(ExprNode expr, LexLocation lexLoc)
        {
            LexLoc = lexLoc;
            Expr = expr;
        }
    }

    public class ActualParams : GrammarType
    {
        // HACK: should be private
        public List<ExprNode> exprList = new List<ExprNode>();

        public int Count { get; set; }

        public void Add(ExprNode expr)
        {
            exprList.Add(expr);
            ++Count;
        }

    }

    public class FormalParams: GrammarType
    {
        public class FormalParam
        {
            public LexLocation LexLoc;

            public Symbol.ValueType Type { get; set; }

            public DeclId Name { get; set; }

            public FormalParam(DeclType type, DeclId name, LexLocation lexLoc)
            {
                LexLoc = lexLoc;
                Type = type.Type;
                Name = name;
            }
        }

        public LexLocation LexLoc;

        public List<FormalParam> FormalParamList = new List<FormalParam>();

        public FormalParams(DeclType type, DeclId name, LexLocation lexLoc)
        {
            FormalParamList.Add(new FormalParam(type, name, name.LexLoc));
        }

        public FormalParams()
        {

        }

        public void Add(DeclType type, DeclId name)
        {
            FormalParamList.Add(new FormalParam(type, name, name.LexLoc));
        }
    }

    public class DeclType: GrammarType
    {
        public LexLocation LexLoc { get; set; }

        public Symbol.ValueType Type { get; set; }

        public DeclType(string type, LexLocation lexLoc)
        {
            LexLoc = lexLoc;
            Symbol t = ParserHelper.GlobalScope.Get(type);
            if (!(t is TypeSymbol))
            {
                throw new SemanticExepction("Недопустимый тип аргумента " + Type 
                    + ". Строка " + LexLoc.StartLine + ", столбец " + LexLoc.StartColumn);
            }
            Type = (t as TypeSymbol).Value;
        }
    }

    public class DeclId: GrammarType
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
        public LexLocation LexLoc { get; set; }

        public Node(LexLocation lexLoc)
        {
            LexLoc = lexLoc;
        }

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
            ParserHelper.GlobalScope.Put(Header.Name, funSymbol);
        }

        public override T Visit<T>(Visitor<T> v)
        {
            return v.Visit(this);
        }
    }

    public abstract class ExprNode : Node // базовый класс для всех выражений
    {
        public ExprNode(LexLocation lexLoc) : base(lexLoc) { }
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
        public StatementNode(LexLocation lexLoc) : base(lexLoc) { }
    }

    public class AssignNode : StatementNode
    {
        public IdNode Id { get; set; }

        public ExprNode Expr { get; set; }

        public AssignType AssOp { get; set; }

        public AssignNode(IdNode id, ExprNode expr, LexLocation lexLoc, AssignType assop = AssignType.Assign) : base(lexLoc)
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

        public CondNode(ExprNode expr, StatementNode statIf, StatementNode statElse, LexLocation lexLoc) : base(lexLoc)
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

        public BlockNode(StatementNode stat, LexLocation lexLoc) : base(lexLoc)
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

        public ProcCallNode(FunCallNode funCall, LexLocation lexLoc) : base(lexLoc)
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
        public FunCallNode(string name, ActualParams actualParams, LexLocation lexLoc) : base(lexLoc)
        {
            Name = name;
            if (actualParams == null)
            {
                ActParams = new ActualParams();
            } else
            {
                ActParams = actualParams;
            }
        }

        public ActualParams ActParams { get; set; }

        public string Name { get; set; }

        public override T Visit<T>(Visitor<T> v)
        {
            return v.Visit(this);
        }
    }

    public class DeclNode : StatementNode {

        public Symbol.ValueType Type { set; get; }

        public DeclList DeclsList { get; set; }

        public DeclNode(DeclType type, DeclList declsList, LexLocation lexLoc) : base(lexLoc)
        {
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

        public ReturnNode(ExprNode expr, LexLocation lexLoc) : base(lexLoc)
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

        public WhileNode(ExprNode expr, StatementNode stat, LexLocation lexLoc) : base(lexLoc)
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

        public DoWhileNode(ExprNode expr, StatementNode stat, LexLocation lexLoc) : base(lexLoc)
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
        public class ForInitializer: StatementNode
        {
            private DeclNode decl;
            private AssignNode assign;
            public string Name { get; set; }

            public ForInitializer(StatementNode stat, LexLocation lexLoc) : base(lexLoc)
            {
                if (stat is DeclNode)
                {
                    decl = stat as DeclNode;
                    Name = decl.DeclsList.Decls[0].Id.Name;
                }
                else if (stat is AssignNode)
                {
                    assign = stat as AssignNode;
                    Name = assign.Id.Name;
                }
                else
                {
                    throw new SemanticExepction("Invalid initialization section in \'for\' cycle", this);
                }
            }

            public override T Visit<T>(Visitor<T> v)
            {
                if (decl != null)
                {
                    return v.Visit(decl);
                }
                else
                {
                    return v.Visit(assign);
                }
            }

        }

        public ForInitializer Init { get; set; }

        public StatementNode Iter { get; set; }

        public ExprNode Cond { get; set; }

        public StatementNode Stat { get; set; }

        public ForNode(ForInitializer init, ExprNode cond, StatementNode iter, StatementNode stat, LexLocation lexLoc) : base(lexLoc)
        {
            Init = init;
            Cond = cond;
            if (!(iter is ProcCallNode || iter is AssignNode))
            {
                throw new SemanticExepction("Invalid iteration section in \'for\' cycle", this);
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