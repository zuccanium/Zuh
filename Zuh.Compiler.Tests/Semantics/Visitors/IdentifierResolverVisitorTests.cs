using Zuh.Compiler.Ast;
using Zuh.Compiler.Diagnostics;
using Zuh.Compiler.Semantics;
using Zuh.Compiler.Semantics.Diagnostics;
using Zuh.Compiler.Semantics.Symbols;
using Zuh.Compiler.Semantics.Visitors;

namespace Zuh.Compiler.Tests.Semantics.Visitors {
    public class IdentifierResolverVisitorTests {
        [Fact]
        public void IdentifierResolverVisitor_Works_WithRootStatements() {
            var referencedSchema = new ExpressionDeclaration() {
                Name = new Label() {
                    Value = "referencedSchema"
                },
                Expression = new SchemaExpression() {
                    Schema = new Schema() {
                        Entries = []
                    }
                }
            };

            var referencedSchemaIdentifierReference = new Identifier() {
                Value = nameof(referencedSchema)
            };

            var referencingSchema = new ExpressionDeclaration() {
                Name = new Label() {
                    Value = "referencingSchema"
                },
                Expression = new SchemaExpression() {
                    Schema = new Schema() {
                        Entries = [
                            new SchemaEntry() {
                                Key = new StaticKey() {
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
                }
            };
            
            var file = new ZuhFile() {
                RootStatements = [
                    referencedSchema,
                    referencingSchema
                ]
            };

            var referencedSchemaSymbol = new ExpressionSymbol() {
                Name = nameof(referencedSchema),
                Expression = referencedSchema.Expression
            };

            var fileScope = new Scope() {
                Symbols = {
                    [nameof(referencedSchema)] = referencedSchemaSymbol,
                    [nameof(referencingSchema)] = new ExpressionSymbol() {
                        Name = nameof(referencingSchema),
                        Expression = referencingSchema.Expression
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
                Type = FunctionParameter.FunctionParameterType.Sum
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
                                    Key = new ExpressionKey() {
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
            
            var schemaParamSymbol = new FunctionParameterSymbol() {
                Name = nameof(schemaParam),
                FunctionParameter = schemaParam
            };
            
            var keysParamSymbol = new FunctionParameterSymbol() {
                Name = nameof(keysParam),
                FunctionParameter = keysParam
            };

            var funcSymbol = new FunctionSymbol() {
                Name = nameof(func),
                Function = func.Function,
                Parameters = [schemaParamSymbol, keysParamSymbol]
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

        [Fact]
        public void Visit_BadIdentifier_CreatesDiagnostic() {
            var identifier = new Identifier() {
                Value = "identifier",
                SourceSpan = new SourceSpan() {
                    Start = 235, // completely arbitrary numbers
                    End = 2903
                }
            };

            var declaration = new ExpressionDeclaration() {
                Name = new Label() {
                    Value = ""
                },
                Expression = new IdentifierExpression() {
                    Identifier = identifier
                }
            };
            
            var file = new ZuhFile() {
                RootStatements = [
                    declaration
                ]
            };

            var fileScope = new Scope();

            var scopeTracker = new ScopeTracker() {
                NodeToPersonalScope = {
                    [file] = fileScope,
                },
                NodeToEnclosingScope = {
                    [declaration] = fileScope,
                    [identifier] = fileScope
                }
            };
            
            var symbolTracker = new SymbolTracker();

            var visitor = new IdentifierResolverVisitor() {
                SymbolTracker = symbolTracker,
                ScopeTracker = scopeTracker
            };
            
            visitor.Visit(file);

            var expectedDiagnostic = new SymbolResolutionError() {
                SymbolName = nameof(identifier),
                Location = identifier.SourceSpan
            };
            
            Assert.Equivalent((List<Diagnostic>)[expectedDiagnostic], visitor.Diagnostics);
        }
    }
}