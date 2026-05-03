using System.Collections.Immutable;
using System.Diagnostics;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Diagnostics;
using Zuh.Compiler.Semantics.Diagnostics;
using Zuh.Compiler.Semantics.Symbols;

namespace Zuh.Compiler.Semantics.Visitors {
    /// <summary>
    /// looks for declarations and adds them to their enclosing scopes.
    /// </summary>
    public class SymbolDeclarationVisitor : Visitor {
        public required ScopeTracker ScopeTracker { get; init; }

        protected override List<Overload> Overloads
            => [
                new Overload<Function>((node, next) => {
                    var personalScope = ScopeTracker.NodeToPersonalScope[node];

                    foreach(var param in node.Parameters)
                        personalScope.Declare(new FunctionParameterSymbol() {
                            Name = param.Name.Value,
                            FunctionParameter = param
                        });

                    next();
                }),
                new Overload<Declaration>((node, next) => {
                    next();
                    
                    var enclosingScope = ScopeTracker.NodeToEnclosingScope[node];

                    var isExport = node.IsExport;
                    var name = node.Name.Value;

                    enclosingScope.Declare(
                        node switch {
                            ExpressionDeclaration expressionDeclarationNode => new ExpressionSymbol() {
                                Name = name,
                                Expression = expressionDeclarationNode.Expression,
                                IsExport = isExport
                            },
                            FunctionDeclaration functionDeclarationNode => new FunctionSymbol() {
                                Name = name,
                                Function = functionDeclarationNode.Function,
                                Parameters = [
                                    ..ScopeTracker.NodeToPersonalScope[functionDeclarationNode.Function].Symbols
                                        .Values
                                        .Cast<FunctionParameterSymbol>()
                                ],
                                IsExport = isExport
                            },
                            _ => throw new UnreachableException()
                        }
                    );
                })
            ];

        // make it handle duplicate declarations eventually
        private void handleDeclarationResult(Result<DeclarationError> result) {
            
        }
    }
}