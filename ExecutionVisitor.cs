using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;
using SimpleParser;
using BinExprExecutionHelper;

namespace SimpleLang
{
    public class ExecutionVisitor : Visitor<VarSymbol>
    {
        public enum State
        {
            RETURN,
            RUN
        }

        private State execState = State.RUN;

        public State ExecState
        {
            get
            {
                return execState;
            }

            set
            {
                execState = value;
            }
        }

        public override VarSymbol Visit(MainProgramNode node)
        {
            ParserHelper.enterFun(); 
            VarSymbol result = node.FunList[node.FunList.Count - 1].Visit(this);
            ParserHelper.leaveFun();
            return result;
        }

        public override VarSymbol Visit(AssignNode node)
        {
            VarSymbol leftValue = ParserHelper.CurrentScope().Get(node.Id.Name) as VarSymbol;
            leftValue.Value = node.Expr.Visit(this).Value;
            Console.WriteLine("{0} := int: {1}, double: {2}, bool: {3}",
                node.Id.Name, leftValue.Value.iValue, leftValue.Value.dValue, leftValue.Value.bValue);
            return null;
        }

        public override VarSymbol Visit(BlockNode node)
        {
            ParserHelper.enterScope();

            VarSymbol returnValue = null;
            foreach (StatementNode st in node.StList)
            {
                returnValue = st.Visit(this);
                if (ExecState == State.RETURN)
                {
                    break;
                }
            }

            ParserHelper.leaveScope();
            return returnValue;
        }

        public override VarSymbol Visit(DeclNode node)
        {
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
            return null;
        }

        public override VarSymbol Visit(FunNode node)
        {
            VarSymbol result = node.Body.Visit(this);
            ExecState = State.RUN;
            return result;
        }

        public override VarSymbol Visit(ReturnNode node)
        {
            VarSymbol toReturn;
            if (node.Expr == null)
            {
                toReturn = new VarSymbol(Symbol.ValueType.VOID);
            } else
            {
                toReturn = node.Expr.Visit(this);
            }
            ExecState = State.RETURN;
            return toReturn;
        }

        public override VarSymbol Visit(BoolNode node)
        {
            return new VarSymbol(node.Val);
        }

        public override VarSymbol Visit(IntNumNode node)
        {
            return new VarSymbol(node.Num);
        }

        public override VarSymbol Visit(DoubleNumNode node)
        {
            return new VarSymbol(node.Num);
        }

        public override VarSymbol Visit(IdNode node)
        {
            VarSymbol s = ParserHelper.CurrentScope().Get(node.Name) as VarSymbol;
            return s;
        }

        public override VarSymbol Visit(UnExprNode node)
        {
            switch(node.Op)
            {
                case OpType.Not:
                    return new VarSymbol(!node.Expr.Visit(this).Value.bValue);
                default:
                    throw new SemanticExepction("Неизвестный унарный оператор", node);
            }
        }

        public override VarSymbol Visit(CondNode node)
        {
            ParserHelper.enterScope();

            VarSymbol toReturn = null;
            if (node.Expr.Visit(this).Value.bValue)
            {
                toReturn = node.StatIf.Visit(this);
            } else
            {
                if (node.StatElse != null)
                {
                    toReturn = node.StatElse.Visit(this);
                }
            }

            ParserHelper.leaveScope();
            return toReturn;
        }

        public override VarSymbol Visit(WhileNode node)
        {
            ParserHelper.enterScope();

            VarSymbol toReturn = null;
            while (node.Expr.Visit(this).Value.bValue)
            {
                toReturn = node.Stat.Visit(this);
                if (ExecState == State.RETURN)
                {
                    break;
                }
            }

            ParserHelper.leaveScope();
            return toReturn;
        }

        public override VarSymbol Visit(DoWhileNode node)
        {
            ParserHelper.enterScope();

            VarSymbol toReturn = null;
            do
            {
                toReturn = node.Stat.Visit(this);
                if (ExecState == State.RETURN)
                {
                    break;
                }
            } while (node.Expr.Visit(this).Value.bValue);

            ParserHelper.leaveScope();
            return toReturn;
        }

        public override VarSymbol Visit(ForNode node)
        {
            ParserHelper.enterScope();

            node.Init.Visit(this);
            VarSymbol initVar = null;
            initVar = ParserHelper.CurrentScope().Get(node.Init.Name) as VarSymbol;

            VarSymbol toReturn = null;
            while (node.Cond.Visit(this).Value.bValue)
            {
                toReturn = node.Stat.Visit(this);
                if (ExecState == State.RETURN)
                {
                    break;
                }
                node.Iter.Visit(this);
            }

            ParserHelper.leaveScope();
            return toReturn;
        }

        public override VarSymbol Visit(FunCallNode node)
        {
            FunSymbol fun = ParserHelper.GlobalScope.Get(node.Name) as FunSymbol;
            FormalParams args = fun.Address.Header.Args;
            List<VarSymbol> callArgs = new List<VarSymbol>();
            foreach (ExprNode expr in node.ActParams.exprList)
            {
                callArgs.Add(expr.Visit(this));
            }

            ParserHelper.Stack.Push(new SymbolsRecord());
            for (int i = 0; i < args.FormalParamList.Count; ++i)
            {
                ParserHelper.CurrentScope().Put(args.FormalParamList[i].Name.Name, callArgs[i]);
            }
            VarSymbol toReturn = fun.Address.Visit(this);
            ParserHelper.Stack.Pop();
            return toReturn;
        }

        public override VarSymbol Visit(ProcCallNode node)
        {
            node.FunCall.Visit(this);
            return null;
        }

        public override VarSymbol Visit(BinExprNode node)
        {
            VarSymbol leftValue = node.Left.Visit(this);
            VarSymbol rightValue = node.Right.Visit(this);
            if (leftValue.Type == Symbol.ValueType.INT && rightValue.Type == Symbol.ValueType.DOUBLE)
            {
                leftValue = ParserHelper.upCast(leftValue, Symbol.ValueType.DOUBLE);
            }
            else if (leftValue.Type == Symbol.ValueType.DOUBLE && rightValue.Type == Symbol.ValueType.INT)
            {
                ParserHelper.upCast(rightValue, Symbol.ValueType.DOUBLE);
            }

            BinOp op = null;
            switch (node.Op)
            {
                case OpType.Plus:
                    op = new PlusOp();
                    break;
                case OpType.Minus:
                    op = new MinusOp();
                    break;
                case OpType.Mult:
                    op = new MultOp();
                    break;
                case OpType.Div:
                    op = new DivOp();
                    break;
                case OpType.And:
                    op = new AndOp();
                    break;
                case OpType.Or:
                    op = new OrOp();
                    break;
                case OpType.Gt:
                    op = new GtOp();
                    break;
                case OpType.Lt:
                    op = new LtOp();
                    break;
                case OpType.Get:
                    op = new GetOp();
                    break;
                case OpType.Let:
                    op = new LetOp();
                    break;
                case OpType.Eq:
                    op = new EqOp();
                    break;
                case OpType.Neq:
                    op = new NeqOp();
                    break;
                default:
                    throw new SemanticExepction("Неизвестный бинарный оператор", node);
            }
            return op.Calculate(leftValue, rightValue);
        }
    }
}
