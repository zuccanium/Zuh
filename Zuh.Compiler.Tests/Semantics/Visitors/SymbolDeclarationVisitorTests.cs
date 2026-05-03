using Zuh.Compiler.Ast;
using Zuh.Compiler.Semantics;
using Zuh.Compiler.Semantics.Visitors;

namespace Zuh.Compiler.Tests.Semantics.Visitors {
    public class SymbolDeclarationVisitorTests {
        [Fact]
        public void SymbolDeclarationVisitor_Works_WithRootStatements() {
            var schema = new SchemaDeclaration() {
                Name = new Label() {
                    Value = "schema"
                },
                Schema = new Schema() {
                    Entries = []
                }
            };
            
            var file = new ZuhFile() {
                RootStatements = [
                    schema
                ]
            };

            var fileScope = new Scope();

            var scopeTracker = new ScopeTracker() {
                NodeToPersonalScope = {
                    [file] = fileScope
                },
                NodeToEnclosingScope = {
                    [schema] = fileScope
                }
            };

            var visitor = new SymbolDeclarationVisitor() {
                ScopeTracker = scopeTracker
            };
            
            visitor.Visit(file);
            
            Assert.True(fileScope.Symbols.TryGetValue(nameof(schema), out var schemaSymbol));
            
            Assert.Equivalent(schemaSymbol, new Symbol() {
                Name = nameof(schema),
                Node = schema,
                Visibility = Symbol.SymbolVisibility.Local
            });
        }

        [Fact]
        public void SymbolDeclarationVisitor_Works_WithFunctionParameters() {
            var schemaParam = new FunctionParameter() {
                Name = new Label() {
                    Value = "schemaParam"
                },
                Type = FunctionParameter.FunctionParameterType.Schema
            };
            
            var keysParam = new FunctionParameter() {
                Name = new Label() {
                    Value = "keysParam"
                },
                Type = FunctionParameter.FunctionParameterType.Keys
            };

            var func = new FunctionDeclaration() {
                Name = new Label() {
                    Value = "func"
                },
                Function = new Function() {
                    Parameters = [
                        schemaParam,
                        keysParam
                    ],
                    Expression = new SchemaExpression() {
                        Schema = new Schema() {
                            Entries = []
                        }
                    }
                }
            };
            
            var file = new ZuhFile() {
                RootStatements = [
                    func
                ]
            };

            var fileScope = new Scope();
            var funcScope = new Scope();

            var scopeTracker = new ScopeTracker() {
                NodeToPersonalScope = {
                    [file] = fileScope,
                    [func.Function] = funcScope
                },
                NodeToEnclosingScope = {
                    [func] = fileScope,
                }
            };
            
            var visitor = new SymbolDeclarationVisitor() {
                ScopeTracker = scopeTracker
            };
            
            visitor.Visit(file);
            
            Assert.True(funcScope.Symbols.TryGetValue(nameof(schemaParam), out var schemaParamSymbol));
            Assert.True(funcScope.Symbols.TryGetValue(nameof(keysParam), out var keysParamSymbol));
            
            Assert.Equivalent(schemaParamSymbol, new Symbol() {
                Name = nameof(schemaParam),
                Node = schemaParam,
                Visibility = Symbol.SymbolVisibility.Local
            });
            
            Assert.Equivalent(keysParamSymbol, new Symbol() {
                Name = nameof(keysParam),
                Node = keysParam,
                Visibility = Symbol.SymbolVisibility.Local
            });
        }
    }
}