using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang
{
    public abstract class AutoVisitor: Visitor
    {
        public override void Visit(AssignNode node)
        {
            node.Expr.Visit(this);
            node.Id.Visit(this);
        }

        public override void Visit(BinExprNode node)
        {
            node.Left.Visit(this);
        }

        public override void Visit(BlockNode node)
        {
            foreach(StatementNode st in node.StList)
            {
                st.Visit(this);
            }
        }
    }
}
