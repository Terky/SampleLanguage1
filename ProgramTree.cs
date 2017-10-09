using System.Collections.Generic;

namespace ProgramTree
{
    public enum AssignType { Assign, AssignPlus, AssignMinus, AssignMult, AssignDivide };

    public enum OpType { Plus, Minus, Div, Mult };

    public class Node // базовый класс для всех узлов    
    {
    }

    public abstract class ExprNode : Node // базовый класс для всех выражений
    {
        public abstract int Eval();
    }

    public class IdNode : ExprNode
    {
        public string Name { get; set; }
        public IdNode(string name) { Name = name; }
        public int Value { get; set; }
        public override int Eval()
        {
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
        public override int Eval()
        {
            int res = Left.Eval();
            switch (Op)
            {
                case OpType.Plus:
                    res += Right.Eval();
                    break;
                case OpType.Minus:
                    res -= Right.Eval();
                    break;
                case OpType.Mult:
                    res *= Right.Eval();
                    break;
                case OpType.Div:
                    res /= Right.Eval();
                    break;
            }
            return res;
        }

    }

    public class IntNumNode : ExprNode
    {
        public int Num { get; set; }
        public IntNumNode(int num) { Num = num; }
        public override int Eval()
        {
            return Num;
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
            Id.Value = Expr.Eval();
            System.Console.WriteLine("{0} := {1}", Id.Name, Id.Value);
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
            System.Console.WriteLine("Cycle {0}", Expr.Eval());
            for (int i = 0; i < Expr.Eval(); ++i)
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
            foreach (StatementNode stNode in StList) {
                stNode.Exec();
            }
        }

    }

}