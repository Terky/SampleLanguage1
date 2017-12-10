using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;
using SimpleParser;

namespace SimpleLang
{
    public class StaticCheckVisitor : Visitor<Symbol.ValueType>
    {

        public override Symbol.ValueType Visit(MainProgramNode node)
        {
            foreach (FunNode fun in node.FunList)
            {
                FunSymbol funSym = new FunSymbol(fun.Header.Type, fun);
                ParserHelper.enterFun();
                fun.Visit(this);
                ParserHelper.leaveFun();
            }
            return Symbol.ValueType.NOT_A_TYPE;
        }

        public override Symbol.ValueType Visit(FunNode node)
        {
            var args = node.Header.Args.FormalParamList;
            foreach (var arg in args)
            {
                VarSymbol sym = new VarSymbol();
                sym.Type = arg.Type;
                ParserHelper.CurrentScope().Put(arg.Name.Name, sym);
            }
            bool hasReturn = false;
            Symbol.ValueType returnType = Symbol.ValueType.UNKNOWN;
            foreach (StatementNode stat in node.Body.StList)
            {
                if (stat is ReturnNode)
                {
                    hasReturn = true;
                    Symbol.ValueType funType = node.Header.Type;
                    //ExprNode returnExpr = (stat as ReturnNode).Expr;
                    //if (returnExpr != null)
                    //{

                    //    returnType = returnExpr.Visit(this);
                    //} else
                    //{
                    //    returnType = Symbol.ValueType.VOID;
                    //}
                    returnType = stat.Visit(this);
                    if (funType != returnType)
                    {
                        throw new IncompatibleTypesException(
                            "Несоответствие возвращаемого и указанного типа в функции " + node.Header.Name, node);
                    }
                }
                else
                {
                    stat.Visit(this);
                }
            }
            if (!hasReturn && node.Header.Type != Symbol.ValueType.VOID)
            {
                throw new SemanticExepction("Пропущено выражение return в функции " + node.Header.Name, node);
            }
            return returnType;
        }

        public override Symbol.ValueType Visit(AssignNode node)
        {
            Symbol.ValueType exprType = node.Expr.Visit(this);
            Symbol.ValueType idType = node.Id.Visit(this);
            //TODO: сделать совместимыми по присваиванию не только равные типы
            if (idType != exprType)
            {
                throw new IncompatibleTypesException("Incompatible assign types: " +
                    idType + " and " + exprType, node);
            }
            return Symbol.ValueType.NOT_A_TYPE;
        }

        public override Symbol.ValueType Visit(BlockNode node)
        {
            ParserHelper.enterScope();
            foreach (StatementNode st in node.StList)
            {
                st.Visit(this);
            }
            ParserHelper.leaveScope();
            return Symbol.ValueType.NOT_A_TYPE;
        }

        public override Symbol.ValueType Visit(CondNode node)
        {
            ParserHelper.enterScope();

            Symbol.ValueType exprType = node.Expr.Visit(this);
            if (exprType != Symbol.ValueType.BOOL)
            {
                //TODO: сделать новый класс исключений для подобного?
                throw new IncompatibleTypesException("Не удалось преобразовать тип " +
                    exprType + " к BOOL", node);
            }
            node.StatIf.Visit(this);
            if (node.StatElse != null)
            {
                node.StatElse.Visit(this);
            }

            ParserHelper.leaveScope();
            return Symbol.ValueType.NOT_A_TYPE;
        }

        public override Symbol.ValueType Visit(DeclNode node)
        {
            if (node.Type == Symbol.ValueType.VOID)
            {
                throw new SemanticExepction("Использование типа void в данном контексте недопустимо", node);
            }
            foreach (var decl in node.DeclsList.Decls)
            {
                VarSymbol s = new VarSymbol();
                s.Type = node.Type;
                ParserHelper.CurrentScope().Put(decl.Id.Name, s);
                if (decl.Assign != null)
                {
                    decl.Assign.Visit(this);
                }
            }
            return Symbol.ValueType.NOT_A_TYPE;
        }

        public override Symbol.ValueType Visit(ReturnNode node)
        {
            Symbol.ValueType returnType;
            if (node.Expr != null)
            {
                returnType = node.Expr.Visit(this);
            }
            else
            {
                returnType = Symbol.ValueType.VOID;
            }
            return returnType;
        }

        public override Symbol.ValueType Visit(BoolNode node)
        {
            return Symbol.ValueType.BOOL;
        }

        public override Symbol.ValueType Visit(DoubleNumNode node)
        {
            return Symbol.ValueType.DOUBLE;
        }

        public override Symbol.ValueType Visit(IntNumNode node)
        {
            return Symbol.ValueType.INT;
        }

        public override Symbol.ValueType Visit(IdNode node)
        {
            VarSymbol sym = ParserHelper.CurrentScope().Get(node.Name) as VarSymbol;
            return sym.Type;
        }

        public override Symbol.ValueType Visit(UnExprNode node)
        {
            switch (node.Op)
            {
                case OpType.Not:
                    Symbol.ValueType exprType = node.Expr.Visit(this);
                    if (exprType != Symbol.ValueType.BOOL)
                    {
                        throw new IncompatibleTypesException("Несоответствие типов, оператор !", node);
                    }
                    return Symbol.ValueType.BOOL;
                default:
                    break;
            }
            return Symbol.ValueType.UNKNOWN;
        }

        public override Symbol.ValueType Visit(FunCallNode node)
        {
            Symbol sym = ParserHelper.GlobalScope.Get(node.Name);
            if (!(sym is FunSymbol))
            {
                throw new SemanticExepction(node.Name + " не является именем функции", node);
            }
            FunSymbol fun = sym as FunSymbol;
            FormalParams args = fun.Address.Header.Args;
            if (args.FormalParamList.Count != node.ActualParams.Count)
            {
                throw new SemanticExepction("Неверное количество параметров при вызове функции " + node.Name, node);
            }

            List<Symbol.ValueType> argsType = new List<Symbol.ValueType>();
            foreach (ExprNode expr in node.ActualParams)
            {
                argsType.Add(expr.Visit(this));
            }

            for (int i = 0; i < args.FormalParamList.Count; ++i)
            {
                if (argsType[i] != args.FormalParamList[i].Type)
                {
                    throw new IncompatibleTypesException("Несоответствие типов в параметре " 
                        + args.FormalParamList[i].Name + " функции " + node.Name, node);
                }
            }
            return fun.Type;
        }

        public override Symbol.ValueType Visit(ProcCallNode node)
        {
            node.FunCall.Visit(this);
            return Symbol.ValueType.NOT_A_TYPE;
        }

        public override Symbol.ValueType Visit(WhileNode node)
        {
            ParserHelper.enterScope();

            if (node.Expr.Visit(this) != Symbol.ValueType.BOOL)
            {
                throw new IncompatibleTypesException("Несоответствие типов в выражении для цикла while", node);
            }
            node.Stat.Visit(this);

            ParserHelper.leaveScope();
            return Symbol.ValueType.NOT_A_TYPE;
        }

        public override Symbol.ValueType Visit(DoWhileNode node)
        {
            ParserHelper.enterScope();

            node.Stat.Visit(this);
            if (node.Expr.Visit(this) != Symbol.ValueType.BOOL)
            {
                throw new IncompatibleTypesException("Несоответствие типов в выражении для цикла do while", node);
            }

            ParserHelper.leaveScope();
            return Symbol.ValueType.NOT_A_TYPE;
        }

        public override Symbol.ValueType Visit(ForNode node)
        {
            ParserHelper.enterScope();

            node.Init.Visit(this);
            if (node.Cond.Visit(this) != Symbol.ValueType.BOOL)
            {
                throw new IncompatibleTypesException("Несоответствие типов в выражении для цикла for", node);
            }
            node.Stat.Visit(this);
            node.Iter.Visit(this);

            ParserHelper.leaveScope();
            return Symbol.ValueType.NOT_A_TYPE;
        }

        public override Symbol.ValueType Visit(BinExprNode node)
        {
            Symbol.ValueType leftType = node.Left.Visit(this);
            Symbol.ValueType rightType = node.Right.Visit(this);

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
                    throw new IncompatibleTypesException("Несоответствие типов, оператор " + node.Op.ToString(), node);
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
                        throw new SemanticExepction("Оператор " + node.Op.ToString() + " применяется к неподходящим типам", node);
                    }
                    return leftType;
                case OpType.Gt:
                case OpType.Lt:
                case OpType.Get:
                case OpType.Let:
                    if (leftType != Symbol.ValueType.INT && leftType != Symbol.ValueType.DOUBLE)
                    {
                        throw new SemanticExepction("Оператор " + node.Op.ToString() + " применяется к неподходящим типам", node);
                    }
                    return Symbol.ValueType.BOOL;
                case OpType.Eq:
                case OpType.Neq:
                    if (leftType != Symbol.ValueType.INT && leftType != Symbol.ValueType.DOUBLE && leftType != Symbol.ValueType.BOOL)
                    {
                        throw new SemanticExepction("Оператор " + node.Op.ToString() + " применяется к неподходящим типам", node);
                    }
                    return Symbol.ValueType.BOOL;
                case OpType.Or:
                case OpType.And:
                    if (leftType != Symbol.ValueType.BOOL)
                    {
                        throw new SemanticExepction("Оператор " + node.Op.ToString() + " применяется к неподходящим типам", node);
                    }
                    return Symbol.ValueType.BOOL;
                default:
                    break;
            }
            return Symbol.ValueType.UNKNOWN;
        }
    }
}
