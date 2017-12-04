//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using ProgramTree;

//namespace SimpleLang
//{
//    public abstract class AutoVisitor: Visitor
//    {
//        public override void Visit(AssignNode node)
//        {
//            node.Expr.Visit(this);
//            node.Id.Visit(this);
//        }

//        public override void Visit(BinExprNode node)
//        {
//            node.Left.Visit(this);
//            node.Right.Visit(this);
//        }

        

//        public override void Visit(DeclNode node)
//        {
//            node.Assign.Visit(this);
//        }
//    }
//}
