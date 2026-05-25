using System.Net.Mail;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Diagnostics;
using Zuh.Compiler.Semantics;
using Zuh.Compiler.Semantics.Analyzers;
using Zuh.Compiler.Semantics.Diagnostics;
using Zuh.Compiler.Semantics.Symbols;
using Zuh.Compiler.Semantics.Trackers.Unit;
using Zuh.Compiler.Semantics.Types;

namespace Zuh.Compiler.Tests.Semantics.Analyzers {
    public class CompilationAnalyzerTests {
        [Fact]
        public void CompilationAnalyzer_Works_WithImports() {
            const string importedFileName = "imported.zuh";
            const string mainFileName = "main.zuh";

            var schema = new ExpressionDeclaration() {
                IsExport = true,
                Name = new Label() {
                    Value = "schema"
                },
                Expression = new SchemaExpression() {
                    Schema = new Schema() {
                        Entries = []
                    }
                }
            };
            
            var importedFile = new ZuhFile() {
                RootStatements = [
                    schema
                ]
            };
            
            var mainFile = new ZuhFile() {
                RootStatements = [
                    new ImportStatement() {
                        Module = new StringLiteral() {
                            Value = importedFileName
                        }
                    }
                ]
            };

            var importHandler = new MockImportResolver() {
                Files = {
                    [importedFileName] = importedFile
                }
            };

            var analyzer = new CompilationAnalyzer() {
                ImportResolver = importHandler,
                UnitAsts = new() {
                    [mainFileName] = mainFile
                }
            };
            
            analyzer.Analyze();
            
            Assert.True(analyzer.UnitAnalyzers.TryGetValue(mainFileName, out var mainFileAnalyzer));
            Assert.True(mainFileAnalyzer.UnitScopeTracker.NodeToPersonalScope.TryGetValue(mainFile, out var mainFileScope));
            Assert.True(mainFileScope.Symbols.TryGetValue(nameof(schema), out var schemaSymbol));

            var expected = new ExpressionDeclarationSymbol() {
                Name = nameof(schema),
                UnitId = importedFileName,
                ExpressionDeclaration = schema,
                Type = new SchemaType(),
                IsExport = true
            };
            
            Assert.Equivalent(expected, schemaSymbol);
        }

        [Fact]
        public void Analyze_CircularImportDependency_Works() {
            var fileA = new ZuhFile() {
                RootStatements = [
                    new ImportStatement() {
                        Module = new StringLiteral() {
                            Value = "fileB"
                        }
                    }
                ]
            };
            
            var fileB = new ZuhFile() {
                RootStatements = [
                    new ImportStatement() {
                        Module = new StringLiteral() {
                            Value = "fileC"
                        }
                    },
                    new ExpressionDeclaration() {
                        IsExport = true,
                        Name = new Label() {
                            Value = "BThing"
                        },
                        Expression = new SchemaExpression() {
                            Schema = new Schema() {
                                Entries = []
                            }
                        }
                    }
                ]
            };
            
            var fileC = new ZuhFile() {
                RootStatements = [
                    new ImportStatement() {
                        Module = new StringLiteral() {
                            Value = "fileB"
                        }
                    },
                    new ExpressionDeclaration() {
                        IsExport = true,
                        Name = new Label() {
                            Value = "CThing"
                        },
                        Expression = new SchemaExpression() {
                            Schema = new Schema() {
                                Entries = []
                            }
                        }
                    }
                ]
            };

            var compilationAnalyzer = new CompilationAnalyzer() {
                ImportResolver = new MockImportResolver() {
                    Files = {
                        [nameof(fileB)] = fileB,
                        [nameof(fileC)] = fileC,
                    }
                },
                UnitAsts = new() {
                    [nameof(fileA)] = fileA
                }
            };
            
            compilationAnalyzer.Analyze();
            
            Assert.True(compilationAnalyzer.UnitAnalyzers.TryGetValue(nameof(fileA), out var fileAUnitAnalyzer));
            Assert.True(compilationAnalyzer.UnitAnalyzers.TryGetValue(nameof(fileB), out var fileBUnitAnalyzer));
            Assert.True(compilationAnalyzer.UnitAnalyzers.TryGetValue(nameof(fileC), out var fileCUnitAnalyzer));
            
            Assert.True(fileAUnitAnalyzer.UnitScopeTracker.NodeToPersonalScope.TryGetValue(fileAUnitAnalyzer.UnitAst, out var fileARootScope));
            Assert.True(fileBUnitAnalyzer.UnitScopeTracker.NodeToPersonalScope.TryGetValue(fileBUnitAnalyzer.UnitAst, out var fileBRootScope));
            Assert.True(fileCUnitAnalyzer.UnitScopeTracker.NodeToPersonalScope.TryGetValue(fileCUnitAnalyzer.UnitAst, out var fileCRootScope));
            
            Assert.True(fileBRootScope.Symbols.TryGetValue("BThing", out var bThingSymbol));
            Assert.True(fileCRootScope.Symbols.TryGetValue("CThing", out var cThingSymbol));

            var fileAExpectedRootScope = new Scope() {
                Symbols = {
                    ["BThing"] = bThingSymbol
                }
            };
            
            var fileBExpectedRootScope = new Scope() {
                Symbols = {
                    ["BThing"] = bThingSymbol,
                    ["CThing"] = cThingSymbol
                }
            };
            
            var fileCExpectedRootScope = new Scope() {
                Symbols = {
                    ["BThing"] = bThingSymbol,
                    ["CThing"] = cThingSymbol
                }
            };
            
            Assert.Equivalent(fileAExpectedRootScope, fileARootScope);
            Assert.Equivalent(fileBExpectedRootScope, fileBRootScope);
            Assert.Equivalent(fileCExpectedRootScope, fileCRootScope);
        }
        
        [Fact]
        public void Analyze_FailedResolution_CreatesDiagnostic() {
            const string mainFileName = "main.zuh";
            const string importedFileName = "imported.zuh";

            var importStatement = new ImportStatement() {
                Module = new StringLiteral() {
                    Value = importedFileName
                }
            };
            
            var mainFile = new ZuhFile() {
                RootStatements = [
                    importStatement
                ]
            };

            var importHandler = new MockImportResolver() {
                Files = {}
            };

            var analyzer = new CompilationAnalyzer() {
                ImportResolver = importHandler,
                UnitAsts = new() {
                    [mainFileName] = mainFile
                }
            };
            
            analyzer.Analyze();
            
            Assert.True(analyzer.UnitAnalyzers.TryGetValue(mainFileName, out var mainFileAnalyzer));

            var expectedDiagnostic = new ModuleResolutionError() {
                ModuleName = importedFileName,
                Location = importStatement.SourceSpan
            };
            
            Assert.Equivalent((List<Diagnostic>)[expectedDiagnostic], mainFileAnalyzer.Diagnostics);
        }
    }
}