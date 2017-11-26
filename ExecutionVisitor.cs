using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;
using SimpleParser;
using BinExprExecutionHelper;

namespace SimpleLang
{
    public class ExecutionVisitor : Visitor
    {

        public ExecutionVisitor()
        {
            returner = new Returner();
        }

        public enum State
        {
            RETURN,
            RUN
        }

        private State execState = State.RUN;

        public Returner returner { get; set; }

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

        public override void Visit(MainProgramNode node)
        {
            ParserHelper.Stack.Push(new SymbolsRecord());
            node.FunList[node.FunList.Count - 1].Visit(this);
            ParserHelper.Stack.Pop();
        }

        public override void Visit(AssignNode node)
        {
            VarSymbol leftValue = ParserHelper.TopTable().Get(node.Id.Name) as VarSymbol;
            node.Expr.Visit(this);
            VarSymbol.VarValue exprVal = returner.Value.Value;
            leftValue.Value = exprVal;
            Console.WriteLine("{0} := int: {1}, double: {2}, bool: {3}",
                node.Id.Name, leftValue.Value.iValue, leftValue.Value.dValue, leftValue.Value.bValue);
        }

        public override void Visit(BlockNode node)
        {
            var savedTable = ParserHelper.TopTable();
            ParserHelper.Stack.Peek().TopTable = new SymbolTable(ParserHelper.TopTable());

            foreach (StatementNode st in node.StList)
            {
                st.Visit(this);
                if (ExecState == State.RETURN)
                {
                    break;
                }
            }

            ParserHelper.Stack.Peek().TopTable = savedTable;
        }

        public override void Visit(DeclNode node)
        {
            VarSymbol s = new VarSymbol();
            s.Type = node.Type;
            ParserHelper.TopTable().Put(node.Name, s);
            if (node.Assign != null)
            {
                node.Assign.Visit(this);
            }
        }

        public override void Visit(FunNode node)
        {
            node.Body.Visit(this);
            ExecState = State.RUN;
        }

        public override void Visit(ReturnNode node)
        {
            if (node.Expr == null)
            {
                returner.Value = new VarSymbol(Symbol.ValueType.VOID);
            } else
            {
                node.Expr.Visit(this);
            }
            ExecState = State.RETURN;
        }

        public override void Visit(BoolNode node)
        {
            returner.Value = new VarSymbol(node.Val);
        }

        public override void Visit(IntNumNode node)
        {
            returner.Value = new VarSymbol(node.Num);
        }

        public override void Visit(DoubleNumNode node)
        {
            returner.Value = new VarSymbol(node.Num);
        }

        public override void Visit(IdNode node)
        {
            VarSymbol s = ParserHelper.TopTable().Get(node.Name) as VarSymbol;
            returner.Value = s;
        }

        public override void Visit(UnExprNode node)
        {
            node.Expr.Visit(this);
            switch(node.Op)
            {
                case OpType.Not:
                    returner.Value = new VarSymbol(!returner.Value.Value.bValue);
                    break;
                default:
                    throw new SemanticExepction("Неизвестный унарный оператор");
            }
        }

        public override void Visit(CondNode node)
        {
            var savedTable = ParserHelper.TopTable();
            ParserHelper.Stack.Peek().TopTable = new SymbolTable(ParserHelper.TopTable());

            node.Expr.Visit(this);
            if (returner.Value.Value.bValue)
            {
                node.StatIf.Visit(this);
            } else
            {
                if (node.StatElse != null)
                {
                    node.StatElse.Visit(this);
                }
            }

            ParserHelper.Stack.Peek().TopTable = savedTable;
        }

        public override void Visit(WhileNode node)
        {
            var savedTable = ParserHelper.TopTable();
            ParserHelper.Stack.Peek().TopTable = new SymbolTable(ParserHelper.TopTable());

            node.Expr.Visit(this);
            while (returner.Value.Value.bValue)
            {
                node.Stat.Visit(this);
                if (ExecState == State.RETURN)
                {
                    break;
                }
                node.Expr.Visit(this);
            }

            ParserHelper.Stack.Peek().TopTable = savedTable;
        }

        public override void Visit(DoWhileNode node)
        {
            var savedTable = ParserHelper.TopTable();
            ParserHelper.Stack.Peek().TopTable = new SymbolTable(ParserHelper.TopTable());

            do
            {
                node.Stat.Visit(this);
                if (ExecState == State.RETURN)
                {
                    break;
                }
                node.Expr.Visit(this);
            } while (returner.Value.Value.bValue);

            ParserHelper.Stack.Peek().TopTable = savedTable;
        }

        public override void Visit(ForNode node)
        {
            var savedTable = ParserHelper.TopTable();
            ParserHelper.Stack.Peek().TopTable = new SymbolTable(ParserHelper.TopTable());

            node.Init.Visit(this);
            VarSymbol initVar = null;
            if (node.Init is DeclNode)
            {
                initVar = ParserHelper.TopTable().Get((node.Init as DeclNode).Name) as VarSymbol;

            }
            else
            {
                initVar = ParserHelper.TopTable().Get((node.Init as AssignNode).Id.Name) as VarSymbol;
            }

            node.Cond.Visit(this);
            VarSymbol condVal = returner.Value;
            while (condVal.Value.bValue)
            {
                node.Stat.Visit(this);
                if (ExecState == State.RETURN)
                {
                    break;
                }
                node.Iter.Visit(this);
                node.Cond.Visit(this);
            }

            ParserHelper.Stack.Peek().TopTable = savedTable;
        }

        public override void Visit(FunCallNode node)
        {
            FunSymbol fun = ParserHelper.GlobalTable.Get(node.Name) as FunSymbol;
            FormalParams args = fun.Address.Header.Args;
            List<VarSymbol> callArgs = new List<VarSymbol>();
            foreach (ExprNode expr in node.ActualParams)
            {
                expr.Visit(this);
                callArgs.Add(returner.Value);
            }

            ParserHelper.Stack.Push(new SymbolsRecord());
            for (int i = 0; i < args.FormalParamList.Count; ++i)
            {
                ParserHelper.TopTable().Put(args.FormalParamList[i].Name.Name, callArgs[i]);
            }
            fun.Address.Visit(this);
            ParserHelper.Stack.Pop();
        }

        public override void Visit(ProcCallNode node)
        {
            node.FunCall.Visit(this);
        }

        public override void Visit(BinExprNode node)
        {
            node.Left.Visit(this);
            VarSymbol leftValue = returner.Value;
            node.Right.Visit(this);
            VarSymbol rightValue = returner.Value;
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
                    throw new SemanticExepction("Неизвестный бинарный оператор");
            }
            returner.Value = op.Calculate(leftValue, rightValue);
        }
    }
}
