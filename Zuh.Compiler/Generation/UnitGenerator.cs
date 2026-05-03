using System.Diagnostics;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Generation.Nodes;
using Zuh.Compiler.Semantics;
using Zuh.Compiler.Semantics.Analyzers;
using Zuh.Compiler.Semantics.Symbols;

namespace Zuh.Compiler.Generation {
    /// <summary>
    /// thing that generates a root node for a specific compilation unit.
    /// </summary>
    public partial class UnitGenerator {
        private Stack<Dictionary<Symbol, INode>> stackFrames = [];
        
        /// <summary>
        /// the unit analyzer to reference for semantic data.
        /// </summary>
        public required UnitAnalyzer Analyzer { get; init; }

        private Dictionary<Symbol, INode>? topStackFrame
            => stackFrames.TryPeek(out var top)
                ? top
                : null;
        
        public MappingNode Generate() {
            var root = new MappingNode();
            
            foreach(var statement in Analyzer.File.RootStatements) {
                if(statement is not ExpressionDeclaration { IsExport: true } expressionDeclaration)
                    continue;
                
                root[expressionDeclaration.Name.Value] = new MappingNode.Value() {
                    Node = expressionToNode(expressionDeclaration.Expression)
                };
            }

            return root;
        }
    }
}