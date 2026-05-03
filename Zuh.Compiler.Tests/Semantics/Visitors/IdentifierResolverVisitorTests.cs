using Zuh.Compiler.Ast;
using Zuh.Compiler.Semantics;
using Zuh.Compiler.Semantics.Visitors;

namespace Zuh.Compiler.Tests.Semantics.Visitors {
    public class IdentifierResolverVisitorTests {
        [Fact]
        public void IdentifierResolverVisitor_Works_WithRootStatements() {
            var referencedSchema = new SchemaDeclaration() {
                Name = new Label() {
                    Value = "referencedSchema"
                },
                Schema = new Schema() {
                    Entries = []
                }
            };

            var referencedSchemaIdentifierReference = new Identifier() {
                Value = nameof(referencedSchema)
            };

            var referencingSchema = new SchemaDeclaration() {
                Name = new Label() {
                    Value = "referencingSchema"
                },
                Schema = new Schema() {
                    Entries = [
                        new SchemaEntry() {
                            Key = new SchemaEntryStaticKey() {
                                Name = new Label() {
                                    Value = ""
                                }
                            },
                            Value = new IdentifierExpression() {
                                Identifier = referencedSchemaIdentifierReference
                            }
                        }
                    ]
                }
            };
            
            var file = new ZuhFile() {
                RootStatements = [
                    referencedSchema,
                    referencingSchema
                ]
            };

            var referencedSchemaSymbol = new Symbol() {
                Name = nameof(referencedSchema),
                Node = referencedSchema,
                Visibility = Symbol.SymbolVisibility.Local
            };

            var fileScope = new Scope() {
                Symbols = {
                    [nameof(referencedSchema)] = referencedSchemaSymbol,
                    [nameof(referencingSchema)] = new Symbol() {
                        Name = nameof(referencingSchema),
                        Node = referencingSchema,
                        Visibility = Symbol.SymbolVisibility.Local
                    }
                }
            };

            var scopeTracker = new ScopeTracker() {
                NodeToPersonalScope = {
                    [file] = fileScope
                },
                NodeToEnclosingScope = {
                    [referencedSchema] = fileScope,
                    [referencingSchema] = fileScope,
                    [referencedSchemaIdentifierReference] = fileScope
                }
            };

            var symbolTracker = new SymbolTracker();

            var visitor = new IdentifierResolverVisitor() {
                ScopeTracker = scopeTracker,
                SymbolTracker = symbolTracker
            };
            
            visitor.Visit(file);
            
            Assert.True(symbolTracker.Symbols.TryGetValue(referencedSchemaIdentifierReference, out var referencedSchemaSymbolFromTracker));
            
            Assert.Equal(referencedSchemaSymbol, referencedSchemaSymbolFromTracker);
        }
        
        [Fact]
        public void IdentifierResolverVisitor_Works_WithFunctions() {
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

            var schemaParamIdentifierReference = new Identifier() {
                Value = nameof(schemaParam)
            };
            
            var keysParamIdentifierReference = new Identifier() {
                Value = nameof(keysParam)
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
                            Entries = [
                                new SchemaEntry() {
                                    Key = new SchemaEntryExpressionKey() {
                                        Expression = new IdentifierExpression() {
                                            Identifier = keysParamIdentifierReference
                                        }
                                    },
                                    Value = new IdentifierExpression() {
                                        Identifier = schemaParamIdentifierReference
                                    }
                                }
                            ]
                        }
                    }
                }
            };

            var file = new ZuhFile() {
                RootStatements = [
                    func
                ]
            };
            
            var schemaParamSymbol = new Symbol() {
                Name = nameof(schemaParam),
                Node = schemaParam,
                Visibility = Symbol.SymbolVisibility.Local
            };
            
            var keysParamSymbol = new Symbol() {
                Name = nameof(keysParam),
                Node = keysParam,
                Visibility = Symbol.SymbolVisibility.Local
            };

            var funcSymbol = new Symbol() {
                Name = nameof(func),
                Node = func,
                Visibility = Symbol.SymbolVisibility.Local
            };

            var fileScope = new Scope() {
                Symbols = {
                    [nameof(func)] = funcSymbol,
                }
            };

            var funcScope = new Scope() {
                Symbols = {
                    [nameof(schemaParam)] = schemaParamSymbol,
                    [nameof(keysParam)] = keysParamSymbol
                }
            };

            var scopeTracker = new ScopeTracker() {
                NodeToPersonalScope = {
                    [file] = fileScope,
                    [func.Function] = funcScope
                },
                NodeToEnclosingScope = {
                    [func] = fileScope,
                    [schemaParamIdentifierReference] = funcScope,
                    [keysParamIdentifierReference] = funcScope
                }
            };

            var symbolTracker = new SymbolTracker();

            var visitor = new IdentifierResolverVisitor() {
                ScopeTracker = scopeTracker,
                SymbolTracker = symbolTracker
            };
            
            visitor.Visit(file);
            
            Assert.True(symbolTracker.Symbols.TryGetValue(schemaParamIdentifierReference, out var schemaParamSymbolFromTracker));
            Assert.True(symbolTracker.Symbols.TryGetValue(keysParamIdentifierReference, out var keysParamSymbolFromTracker));
            
            Assert.Equal(schemaParamSymbol, schemaParamSymbolFromTracker);
            Assert.Equal(keysParamSymbol, keysParamSymbolFromTracker);
        }
    }
}