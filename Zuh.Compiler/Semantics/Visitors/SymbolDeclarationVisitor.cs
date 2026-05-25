using System.Collections.Immutable;
using System.Diagnostics;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Diagnostics;
using Zuh.Compiler.Semantics.Diagnostics;
using Zuh.Compiler.Semantics.Symbols;
using Zuh.Compiler.Semantics.Trackers.Unit;
using Zuh.Compiler.Utils;

namespace Zuh.Compiler.Semantics.Visitors {
    /// <summary>
    /// looks for declarations and adds them to their enclosing scopes.
    /// </summary>
    public class SymbolDeclarationVisitor : Visitor {
        public required UnitScopeTracker UnitScopeTracker { get; init; }
        public required UnitSymbolTracker UnitSymbolTracker { get; init; }

        public required string UnitId { get; init; }

        public required DiagnosticCollector Diagnostics { get; init; }

        protected override List<Overload> Overloads
            => [
                // handles function parameters (not functions)
                new Overload<Function>((node, next) => {
                    var personalScope = UnitScopeTracker.NodeToPersonalScope[node];

                    foreach(var param in node.Parameters)
                        declare(
                            param,
                            personalScope,
                            new FunctionParameterSymbol() {
                                Name = param.Name.Value,
                                UnitId = UnitId,
                                FunctionParameter = param
                            }
                        );

                    next();
                }),
                new Overload<Declaration>((node, next) => {
                    next();
                    
                    var enclosingScope = UnitScopeTracker.NodeToEnclosingScope[node];

                    var isExport = node.IsExport;
                    var name = node.Name.Value;

                    declare(
                        node,
                        enclosingScope,
                        node switch {
                            ExpressionDeclaration expressionDeclarationNode => new ExpressionDeclarationSymbol() {
                                Name = name,
                                UnitId = UnitId,
                                ExpressionDeclaration = expressionDeclarationNode,
                                IsExport = isExport
                            },
                            FunctionDeclaration functionDeclarationNode => new FunctionDeclarationSymbol() {
                                Name = name,
                                UnitId = UnitId,
                                FunctionDeclaration = functionDeclarationNode,
                                Parameters = [
                                    ..UnitScopeTracker.NodeToPersonalScope[functionDeclarationNode.Function].Symbols
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

        private void declare(ZuhNode node, Scope scope, Symbol symbol) {
            if(!scope.Declare(symbol)) {
                Diagnostics.Add(new DeclarationError() {
                    DeclarationName = symbol.Name,
                    Location = node.SourceSpan
                });

                return;
            }

            UnitSymbolTracker.NodeToPersonalSymbol[node] = symbol;
        }
    }
}