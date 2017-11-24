using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;
using SimpleParser;

namespace SimpleLang
{
    public class StaticTypeCheckVisitor : Visitor
    {
        public Returner returner { get; set; }

        public StaticTypeCheckVisitor()
        {
            returner = new Returner();
        }

        public override void Visit(MainProgramNode node)
        {
            foreach (FunNode fun in node.FunList)
            {
                FunSymbol funSym = new FunSymbol(fun.Header.Type, fun);

                ParserHelper.Stack.Push(new SymbolsRecord());
                fun.Visit(this);
                ParserHelper.Stack.Pop();
            }
        }

        public override void Visit(FunNode node)
        {
            var args = node.Header.Args.FormalParamList;
            foreach (var arg in args)
            {
                VarSymbol sym = new VarSymbol();
                sym.Type = arg.Type;
                ParserHelper.TopTable().Put(arg.Name, sym);
            }
            bool hasReturn = false;
            foreach (StatementNode stat in node.Body.StList)
            {
                if (stat is ReturnNode)
                {
                    hasReturn = true;
                    Symbol.ValueType funType = node.Header.Type;
                    ExprNode returnExpr = (stat as ReturnNode).Expr;
                    Symbol.ValueType returnType;
                    if (returnExpr != null)
                    {
                        returnExpr.Visit(this);
                        returnType = returner.Value.Type;
                    } else
                    {
                        returnType = Symbol.ValueType.VOID;
                    }
                    if (funType != returnType)
                    {
                        throw new IncompatibleTypesException("Несоответствие возвращаемого и указанного типа в функции " + node.Header.Name);
                    }
                }
                else
                {
                    stat.Visit(this);
                }
            }
            if (!hasReturn && node.Header.Type != Symbol.ValueType.VOID)
            {
                throw new SemanticExepction("Пропущено выражение return в функции " + node.Header.Name);
            }
        }

        public override void Visit(AssignNode node)
        {
            node.Expr.Visit(this);
            Symbol.ValueType exprType = returner.Value.Type;
            node.Id.Visit(this);
            Symbol.ValueType idType = returner.Value.Type;
            //TODO: сделать совместимыми по присваиванию не только равные типы
            if (idType != exprType)
            {
                throw new IncompatibleTypesException("Incompatible assign types: " +
                    idType + " and " + exprType);
            }
        }

        public override void Visit(BlockNode node)
        {
            var savedTable = ParserHelper.TopTable();
            ParserHelper.Stack.Peek().TopTable = new SymbolTable(ParserHelper.TopTable());

            foreach (StatementNode st in node.StList)
            {
                st.Visit(this);
            }

            ParserHelper.Stack.Peek().TopTable = savedTable;
        }

        public override void Visit(CondNode node)
        {
            var savedTable = ParserHelper.TopTable();
            ParserHelper.Stack.Peek().TopTable = new SymbolTable(ParserHelper.TopTable());

            node.Expr.Visit(this);
            Symbol.ValueType exprType = returner.Value.Type;
            if (exprType != Symbol.ValueType.BOOL)
            {
                //TODO: сделать новый класс исключений для подобного?
                throw new IncompatibleTypesException("Не удалось преобразовать тип " +
                    exprType + " к BOOL");
            }
            node.StatIf.Visit(this);
            if (node.StatElse != null)
            {
                node.StatElse.Visit(this);
            }

            ParserHelper.Stack.Peek().TopTable = savedTable;
        }

        public override void Visit(DeclNode node)
        {
            if (node.Type == Symbol.ValueType.VOID)
            {
                throw new SemanticExepction("Использование типа void в данном контексте недопустимо");
            }
            VarSymbol s = new VarSymbol();
            s.Type = node.Type;
            ParserHelper.TopTable().Put(node.Name, s);
            if (node.Assign != null)
            {
                node.Assign.Visit(this);
            }
        }

        public override void Visit(ReturnNode node)
        {
            if (node.Expr != null)
            {
                node.Expr.Visit(this);
            }
            else
            {
                returner.Value = new VarSymbol(Symbol.ValueType.VOID);
            }
        }

        public override void Visit(BoolNode node)
        {
            returner.Value = new VarSymbol(Symbol.ValueType.BOOL);
        }

        public override void Visit(DoubleNumNode node)
        {
            returner.Value = new VarSymbol(Symbol.ValueType.DOUBLE);
        }

        public override void Visit(IntNumNode node)
        {
            returner.Value = new VarSymbol(Symbol.ValueType.INT);
        }

        public override void Visit(IdNode node)
        {
            VarSymbol sym = ParserHelper.TopTable().Get(node.Name) as VarSymbol;
            returner.Value = new VarSymbol(sym.Type);
        }

        public override void Visit(UnExprNode node)
        {
            switch (node.Op)
            {
                case OpType.Not:
                    node.Expr.Visit(this);
                    Symbol.ValueType exprType = returner.Value.Type;
                    if (exprType != Symbol.ValueType.BOOL)
                    {
                        throw new IncompatibleTypesException("Несоответствие типов, оператор !");
                    }
                    returner.Value = new VarSymbol(Symbol.ValueType.BOOL);
                    break;
                default:
                    break;
            }
        }

        public override void Visit(FunCallNode node)
        {
            Symbol sym = ParserHelper.GlobalTable.Get(node.Name);
            if (!(sym is FunSymbol))
            {
                throw new SemanticExepction(node.Name + " не является именем функции");
            }
            FunSymbol fun = sym as FunSymbol;
            FormalParams args = fun.Address.Header.Args;
            if (args.FormalParamList.Count != node.ExprList.Count)
            {
                throw new SemanticExepction("Неверное количество параметров при вызове функции " + node.Name);
            }
            List<Symbol.ValueType> argsType = new List<Symbol.ValueType>();
            foreach (ExprNode expr in node.ExprList)
            {
                expr.Visit(this);
                argsType.Add(returner.Value.Type);
            }
            for (int i = 0; i < args.FormalParamList.Count; ++i)
            {
                if (argsType[i] != args.FormalParamList[i].Type)
                {
                    throw new IncompatibleTypesException("Несоответствие типов в параметре " + args.FormalParamList[i].Name + " функции " + node.Name);
                }
            }
        }

        public override void Visit(ProcCallNode node)
        {
            node.FunCall.Visit(this);
        }

        public override void Visit(WhileNode node)
        {
            var savedTable = ParserHelper.TopTable();
            ParserHelper.Stack.Peek().TopTable = new SymbolTable(ParserHelper.TopTable());

            node.Expr.Visit(this);
            if (returner.Value.Type != Symbol.ValueType.BOOL)
            {
                throw new IncompatibleTypesException("Несоответствие типов в выражении для цикла while");
            }
            node.Stat.Visit(this);

            ParserHelper.Stack.Peek().TopTable = savedTable;
        }

        public override void Visit(DoWhileNode node)
        {
            var savedTable = ParserHelper.TopTable();
            ParserHelper.Stack.Peek().TopTable = new SymbolTable(ParserHelper.TopTable());

            node.Stat.Visit(this);
            node.Expr.Visit(this);
            if (returner.Value.Type != Symbol.ValueType.BOOL)
            {
                throw new IncompatibleTypesException("Несоответствие типов в выражении для цикла do while");
            }

            ParserHelper.Stack.Peek().TopTable = savedTable;
        }

        public override void Visit(ForNode node)
        {
            var savedTable = ParserHelper.TopTable();
            ParserHelper.Stack.Peek().TopTable = new SymbolTable(ParserHelper.TopTable());

            node.Init.Visit(this);
            node.Cond.Visit(this);
            if (returner.Value.Type != Symbol.ValueType.BOOL)
            {
                throw new IncompatibleTypesException("Несоответствие типов в выражении для цикла do while");
            }
            node.Stat.Visit(this);
            node.Iter.Visit(this);

            ParserHelper.Stack.Peek().TopTable = savedTable;
        }

        public override void Visit(BinExprNode node)
        {
            node.Left.Visit(this);
            Symbol.ValueType leftType = returner.Value.Type;
            node.Right.Visit(this);
            Symbol.ValueType rightType = returner.Value.Type;

            if (leftType != rightType)
            {
                if ((leftType == Symbol.ValueType.INT && rightType == Symbol.ValueType.DOUBLE) ||
                    (rightType == Symbol.ValueType.INT && leftType == Symbol.ValueType.DOUBLE))
                {
                    leftType = Symbol.ValueType.DOUBLE;
                    rightType = Symbol.ValueType.DOUBLE;
                }
                else
                {
                    throw new IncompatibleTypesException("Несоответствие типов, оператор " + node.Op.ToString());
                }
            }

            switch (node.Op)
            {
                case OpType.Plus:
                case OpType.Minus:
                case OpType.Mult:
                case OpType.Div:
                    if (leftType != Symbol.ValueType.INT && leftType != Symbol.ValueType.DOUBLE)
                    {
                        throw new SemanticExepction("Оператор " + node.Op.ToString() + " применяется к неподходящим типам");
                    }
                    returner.Value = new VarSymbol(leftType);
                    break;
                case OpType.Gt:
                case OpType.Lt:
                case OpType.Get:
                case OpType.Let:
                    if (leftType != Symbol.ValueType.INT && leftType != Symbol.ValueType.DOUBLE)
                    {
                        throw new SemanticExepction("Оператор " + node.Op.ToString() + " применяется к неподходящим типам");
                    }
                    returner.Value = new VarSymbol(Symbol.ValueType.BOOL);
                    break;
                case OpType.Eq:
                case OpType.Neq:
                    if (leftType != Symbol.ValueType.INT && leftType != Symbol.ValueType.DOUBLE && leftType != Symbol.ValueType.BOOL)
                    {
                        throw new SemanticExepction("Оператор " + node.Op.ToString() + " применяется к неподходящим типам");
                    }
                    returner.Value = new VarSymbol(Symbol.ValueType.BOOL);
                    break;
                case OpType.Or:
                case OpType.And:
                    if (leftType != Symbol.ValueType.BOOL)
                    {
                        throw new SemanticExepction("Оператор " + node.Op.ToString() + " применяется к неподходящим типам");
                    }
                    returner.Value = new VarSymbol(Symbol.ValueType.BOOL);
                    break;
                default:
                    break;
            }
        }
    }
}
