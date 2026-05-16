using System.Diagnostics;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Diagnostics;
using Zuh.Compiler.Semantics.Analyzers;
using Zuh.Compiler.Semantics.Diagnostics;

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
                        throw new Exception("failed to get identifier source span?");

                    var resolveResult = nodeScope.Resolve(node.Value);

                    if(resolveResult is { } resolvedSymbol)
                        SymbolTracker.Symbols[node] = resolvedSymbol;
                    
                    else
                        Diagnostics.Add(new SymbolResolutionError() {
                            SymbolName = node.Value,
                            Location = node.SourceSpan
                        });

                    next();
                })
            ];
    }
}