using Zuh.Compiler.Ast;
using Zuh.Compiler.Diagnostics;
using Zuh.Compiler.Semantics.Analyzers;

namespace Zuh.Compiler.Semantics.Visitors {
    /// <summary>
    /// looks for identifiers and adds them to a <see cref="SymbolTracker"/>.
    /// </summary>
    public class IdentifierResolverVisitor : Visitor {
        public required ScopeTracker ScopeTracker { get; init; }
        public required SymbolTracker SymbolTracker { get; init; }
        
        public DiagnosticCollector Diagnostics { get; private init; } = [];

        protected override List<Overload> Overloads
            => [
                new Overload<Identifier>((node, next) => {
                    if(!ScopeTracker.NodeToEnclosingScope.TryGetValue(node, out var nodeScope))
                        throw new Exception("qha??");
            
                    var resolveResult = nodeScope.Resolve(node.Value);

                    if(resolveResult.Diagnostic is { } diagnostic)
                        Diagnostics.Add(diagnostic);

                    SymbolTracker.Symbols[node] = resolveResult.Value!;

                    next();
                })
            ];
    }
}