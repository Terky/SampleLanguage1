using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;
using SimpleParser;

namespace SimpleLang
{
    public class ExecutionVisitor : Visitor
    {
        public Returner returner { get; set; }

        public override void Visit(MainProgramNode node)
        {
            ParserHelper.Stack.Push(new SymbolsRecord());
            node.FunList[node.FunList.Count - 1].Visit(this);
            ParserHelper.Stack.Pop();
        }


    }
}
