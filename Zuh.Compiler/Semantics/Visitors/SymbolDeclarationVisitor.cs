using Zuh.Compiler.Ast;
using Zuh.Compiler.Diagnostics;
using Zuh.Compiler.Semantics.Diagnostics;

namespace Zuh.Compiler.Semantics.Visitors {
    /// <summary>
    /// look for declarations and populates enclosing scopes with them.
    /// </summary>
    public class SymbolDeclarationVisitor : Visitor {
        public required ScopeTracker ScopeTracker { get; init; }

        protected override List<Overload> Overloads
            => [
                new Overload<Function>((node, next) => {
                    var personalScope = ScopeTracker.NodeToPersonalScope[node];
            
                    foreach(var param in node.Parameters)
                        personalScope.Declare(new Symbol() {
                            Name = param.Name.Value,
                            Node = param,
                            Visibility = Symbol.SymbolVisibility.Local
                        });

                    next();
                }),
                new Overload<Declaration>((node, next) => {
                    var enclosingScope = ScopeTracker.NodeToEnclosingScope[node];
            
                    enclosingScope.Declare(new Symbol() {
                        Name = node.Name.Value,
                        Node = node,
                        Visibility = node.IsExport
                            ? Symbol.SymbolVisibility.Exported
                            : Symbol.SymbolVisibility.Local
                    });

                    next();
                })
            ];

        // make it handle duplicate declarations eventually
        private void handleDeclarationResult(Result<DeclarationError> result) {
            
        }
    }
}