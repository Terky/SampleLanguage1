using ProgramTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    public abstract class Visitor
    {
        public abstract void Visit(MainProgramNode node);
        public abstract void Visit(FunNode node);
        public abstract void Visit(IdNode node);
        public abstract void Visit(BinExprNode node);
        public abstract void Visit(UnExprNode node);
        public abstract void Visit(IntNumNode node);
        public abstract void Visit(DoubleNumNode node);
        public abstract void Visit(BoolNode node);
        public abstract void Visit(AssignNode node);
        public abstract void Visit(CondNode node);
        public abstract void Visit(BlockNode node);
        public abstract void Visit(ProcCallNode node);
        public abstract void Visit(FunCallNode node);
        public abstract void Visit(DeclNode node);
        public abstract void Visit(ReturnNode node);
        public abstract void Visit(WhileNode node);
        public abstract void Visit(DoWhileNode node);
        public abstract void Visit(ForNode node);
    }
}
