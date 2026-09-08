using Zuh.Compiler.Ast;
using Zuh.Compiler.Diagnostics;
using Zuh.Compiler.Semantics;
using Zuh.Compiler.Semantics.Diagnostics;
using Zuh.Compiler.Semantics.Symbols;
using Zuh.Compiler.Semantics.Trackers.Unit;
using Zuh.Compiler.Semantics.Visitors;

namespace Zuh.Compiler.Tests.Semantics.Visitors {
    public class SymbolDeclarationVisitorTests {
        private static readonly SourceSpan arbitrarySourceSpan
            = new() {
                Start = 10,
                End = 11
            };
        
        [Fact]
        public void SymbolDeclarationVisitor_Works_WithRootStatements() {
            var schema = new ExpressionDeclaration() {
                Name = new Label() {
                    Value = "schema"
                },
                Expression = new SchemaExpression() {
                    Schema = new Schema() {
                        Entries = []
                    }
                }
            };
            
            var file = new ZuhFile() {
                RootStatements = [
                    schema
                ]
            };

            var fileScope = new Scope();

            var scopeTracker = new UnitScopeTracker() {
                NodeToPersonalScope = {
                    [file] = fileScope
                },
                NodeToEnclosingScope = {
                    [schema] = fileScope
                }
            };

            var symbolTracker = new UnitSymbolTracker();

            var visitor = new SymbolDeclarationVisitor() {
                UnitScopeTracker = scopeTracker,
                UnitSymbolTracker = symbolTracker,
                UnitId = "",
                Diagnostics = null!
            };
            
            visitor.Visit(file);
            
            Assert.True(fileScope.Symbols.TryGetValue(nameof(schema), out var schemaSymbol));
            
            Assert.Equivalent(schemaSymbol, new ExpressionDeclarationSymbol() {
                Name = nameof(schema),
                UnitId = "",
                ExpressionDeclaration = schema
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
                Type = FunctionParameter.FunctionParameterType.Sum
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

            var scopeTracker = new UnitScopeTracker() {
                NodeToPersonalScope = {
                    [file] = fileScope,
                    [func.Function] = funcScope
                },
                NodeToEnclosingScope = {
                    [func] = fileScope,
                }
            };

            var symbolTracker = new UnitSymbolTracker();
            
            var visitor = new SymbolDeclarationVisitor() {
                UnitScopeTracker = scopeTracker,
                UnitSymbolTracker = symbolTracker,
                UnitId = "",
                Diagnostics = null!
            };
            
            visitor.Visit(file);
            
            Assert.True(funcScope.Symbols.TryGetValue(nameof(schemaParam), out var schemaParamSymbol));
            Assert.True(funcScope.Symbols.TryGetValue(nameof(keysParam), out var keysParamSymbol));
            
            Assert.Equivalent(schemaParamSymbol, new FunctionParameterSymbol() {
                Name = nameof(schemaParam),
                UnitId = "",
                FunctionParameter = schemaParam
            });
            
            Assert.Equivalent(keysParamSymbol, new FunctionParameterSymbol() {
                Name = nameof(keysParam),
                UnitId = "",
                FunctionParameter = keysParam
            });
        }

        [Fact]
        public void Visit_DuplicateExpressionDeclaration_CreatesDiagnostic() {
            var declarationName = "declaration";
            
            var firstDeclaration = new ExpressionDeclaration() {
                Name = new Label() {
                    Value = declarationName
                },
                Expression = new SchemaExpression() {
                    Schema = new Schema() {
                        Entries = []
                    }
                }
            };

            var secondDeclaration = firstDeclaration with {
                SourceSpan = arbitrarySourceSpan
            };
            
            var file = new ZuhFile() {
                RootStatements = [
                    firstDeclaration,
                    secondDeclaration
                ]
            };

            var fileScope = new Scope();

            var scopeTracker = new UnitScopeTracker() {
                NodeToPersonalScope = {
                    [file] = fileScope
                },
                NodeToEnclosingScope = {
                    [firstDeclaration] = fileScope,
                    [secondDeclaration] = fileScope
                }
            };

            var symbolTracker = new UnitSymbolTracker();

            var diagnostics = new DiagnosticCollector();
            
            var visitor = new SymbolDeclarationVisitor() {
                UnitScopeTracker = scopeTracker,
                UnitSymbolTracker = symbolTracker,
                UnitId = "",
                Diagnostics = diagnostics
            };
            
            visitor.Visit(file);

            var expectedDiagnostic = new DeclarationError() {
                DeclarationName = declarationName,
                Location = secondDeclaration.SourceSpan
            };
            
            Assert.Equivalent((List<Diagnostic>)[expectedDiagnostic], diagnostics);
        }
        
        [Fact]
        public void Visit_DuplicateParameterDeclaration_CreatesDiagnostic() {
            var parameterName = "parameter";
            
            var firstParameter = new FunctionParameter() {
                Name = new Label() {
                    Value = parameterName
                },
                Type = FunctionParameter.FunctionParameterType.Schema
            };

            var secondParameter = firstParameter with {
                SourceSpan = arbitrarySourceSpan
            };

            var func = new FunctionDeclaration() {
                Name = new Label() {
                    Value = ""
                },
                Function = new Function() {
                    Parameters = [
                        firstParameter,
                        secondParameter
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

            var scopeTracker = new UnitScopeTracker() {
                NodeToPersonalScope = {
                    [file] = fileScope,
                    [func.Function] = funcScope
                },
                NodeToEnclosingScope = {
                    [func] = fileScope
                }
            };

            var symbolTracker = new UnitSymbolTracker();

            var diagnostics = new DiagnosticCollector();
            
            var visitor = new SymbolDeclarationVisitor() {
                UnitScopeTracker = scopeTracker,
                UnitSymbolTracker = symbolTracker,
                UnitId = "",
                Diagnostics = diagnostics
            };
            
            visitor.Visit(file);

            var expectedDiagnostic = new DeclarationError() {
                DeclarationName = parameterName,
                Location = secondParameter.SourceSpan
            };
            
            Assert.Equivalent((List<Diagnostic>)[expectedDiagnostic], diagnostics);
        }
    }
}