using ProgramTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleLang
{
    public abstract class Visitor<T>
    {
        public abstract T Visit(MainProgramNode node);
        public abstract T Visit(FunNode node);
        public abstract T Visit(IdNode node);
        public abstract T Visit(BinExprNode node);
        public abstract T Visit(UnExprNode node);
        public abstract T Visit(IntNumNode node);
        public abstract T Visit(DoubleNumNode node);
        public abstract T Visit(BoolNode node);
        public abstract T Visit(AssignNode node);
        public abstract T Visit(CondNode node);
        public abstract T Visit(BlockNode node);
        public abstract T Visit(ProcCallNode node);
        public abstract T Visit(FunCallNode node);
        public abstract T Visit(DeclNode node);
        public abstract T Visit(ReturnNode node);
        public abstract T Visit(WhileNode node);
        public abstract T Visit(DoWhileNode node);
        public abstract T Visit(ForNode node);
    }
}
