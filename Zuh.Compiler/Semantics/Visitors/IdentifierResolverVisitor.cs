using System.Diagnostics;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Diagnostics;
using Zuh.Compiler.Semantics.Analyzers;
using Zuh.Compiler.Semantics.Diagnostics;
using Zuh.Compiler.Semantics.Trackers.Unit;

namespace Zuh.Compiler.Semantics.Visitors {
    /// <summary>
    /// looks for identifiers and adds them to a <see cref="UnitSymbolTracker"/>.
    /// </summary>
    public class IdentifierResolverVisitor : Visitor {
        public required UnitScopeTracker UnitScopeTracker { get; init; }
        public required UnitSymbolTracker UnitSymbolTracker { get; init; }
        
        public required DiagnosticCollector Diagnostics { get; init; }

        protected override List<Overload> Overloads
            => [
                new Overload<Identifier>((node, next) => {
                    if(!UnitScopeTracker.NodeToEnclosingScope.TryGetValue(node, out var nodeScope))
                        throw new Exception("failed to get identifier source span?");

                    var resolveResult = nodeScope.Resolve(node.Value);

                    if(resolveResult is { } resolvedSymbol)
                        UnitSymbolTracker.IdentifierToSymbol[node] = resolvedSymbol;
                    
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