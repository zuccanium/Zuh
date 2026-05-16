using System.Net.Mail;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Diagnostics;
using Zuh.Compiler.Semantics;
using Zuh.Compiler.Semantics.Analyzers;
using Zuh.Compiler.Semantics.Diagnostics;
using Zuh.Compiler.Semantics.Symbols;

namespace Zuh.Compiler.Tests.Semantics {
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
            Assert.True(mainFileAnalyzer.ScopeTracker.NodeToPersonalScope.TryGetValue(mainFile, out var mainFileScope));
            Assert.True(mainFileScope.Symbols.TryGetValue(nameof(schema), out var schemaSymbol));

            Assert.Equivalent(
                schemaSymbol,
                new ExpressionSymbol() {
                    Name = nameof(schema),
                    Expression = schema.Expression
                }
            );
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