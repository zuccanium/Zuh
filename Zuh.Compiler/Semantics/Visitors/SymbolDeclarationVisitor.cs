using System.Diagnostics;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Diagnostics;
using Zuh.Compiler.Semantics.Diagnostics;
using Zuh.Compiler.Semantics.Symbols;

namespace Zuh.Compiler.Semantics.Visitors {
    /// <summary>
    /// look for declarations and populates enclosing scopes with them.
    /// </summary>
    public class SymbolDeclarationVisitor : Visitor {
        public required ScopeTracker ScopeTracker { get; init; }

        // TODO: FIGURE OUT HOW TO GIVE THE FUNCTION ITS PARAMETER SYMBOLS!!!
        protected override List<Overload> Overloads
            => [
                new Overload<Function>((node, next) => {
                    var personalScope = ScopeTracker.NodeToPersonalScope[node];

                    foreach(var param in node.Parameters)
                        personalScope.Declare(new FunctionParameterSymbol() {
                            Name = param.Name.Value,
                            FunctionParameter = param,
                            Visibility = Symbol.SymbolVisibility.Local
                        });

                    next();
                }),
                new Overload<Declaration>((node, next) => {
                    next();
                    
                    var enclosingScope = ScopeTracker.NodeToEnclosingScope[node];

                    var visibility = node.IsExport
                        ? Symbol.SymbolVisibility.Exported
                        : Symbol.SymbolVisibility.Local;

                    var name = node.Name.Value;

                    enclosingScope.Declare(
                        node switch {
                            SchemaDeclaration schemaDeclarationNode => new SchemaSymbol() {
                                Name = name,
                                Schema = schemaDeclarationNode.Schema,
                                Visibility = visibility
                            },
                            KeysDeclaration keysDeclarationNode => new KeysSymbol() {
                                Name = name,
                                Keys = keysDeclarationNode.Keys,
                                Visibility = visibility
                            },
                            FunctionDeclaration functionDeclarationNode => new FunctionSymbol() {
                                Name = name,
                                Function = functionDeclarationNode.Function,
                                Parameters = [],
                                Visibility = visibility
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