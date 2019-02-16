using System;
using System.Collections.Generic;
using System.Linq;
using OldRod.Core.Disassembly.ControlFlow;

namespace OldRod.Core.Ast.IL
{
    public class ILCompilationUnit : ILAstNode
    {
        private readonly IDictionary<string, ILVariable> _variables = new Dictionary<string, ILVariable>();

        public ILCompilationUnit(ControlFlowGraph controlFlowGraph)
        {
            ControlFlowGraph = controlFlowGraph;
        }
        
        public ICollection<ILVariable> Variables => _variables.Values;

        public ControlFlowGraph ControlFlowGraph
        {
            get;
        }

        public ILVariable GetOrCreateVariable(string name)
        {
            if (!_variables.TryGetValue(name, out var variable))
                _variables.Add(name, variable = new ILVariable(name));
            return variable;
        }

        public void RemoveNonUsedVariables()
        {
            foreach (var entry in _variables.ToArray())
            {
                if (entry.Value.UsedBy.Count == 0)
                    _variables.Remove(entry.Key);
            }
        }

        public override void ReplaceNode(ILAstNode node, ILAstNode newNode)
        {
            throw new NotSupportedException();
        }

        public override void AcceptVisitor(IILAstVisitor visitor)
        {
            visitor.VisitCompilationUnit(this);
        }
        
        public override TResult AcceptVisitor<TResult>(IILAstVisitor<TResult> visitor)
        {
            return visitor.VisitCompilationUnit(this);
        }
    }
}