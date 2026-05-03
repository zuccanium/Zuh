using Zuh.Compiler.Ast;
using Zuh.Compiler.Semantics;
using Zuh.Compiler.Semantics.Analyzers;

namespace Zuh.Compiler.Tests.Semantics {
    public class CompilationAnalyzerTests {
        [Fact]
        public void CompilationAnalyzer_Works_WithImports() {
            const string importedFileName = "imported.zuh";
            const string mainFileName = "main.zuh";

            var schema = new SchemaDeclaration() {
                IsExport = true,
                Name = new Label() {
                    Value = "schema"
                },
                Schema = new Schema() {
                    Entries = []
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

            var importHandler = new MockImportHandler() {
                Files = {
                    [importedFileName] = importedFile
                }
            };

            var analyzer = new CompilationAnalyzer() {
                ImportHandler = importHandler,
                Files = new() {
                    [mainFileName] = mainFile
                }
            };
            
            analyzer.Analyze();
            
            Assert.True(analyzer.Analyzers.TryGetValue(mainFileName, out var mainFileAnalyzer));
            Assert.True(mainFileAnalyzer.ScopeTracker.NodeToPersonalScope.TryGetValue(mainFile, out var mainFileScope));
            Assert.True(mainFileScope.Symbols.TryGetValue(nameof(schema), out var schemaSymbol));

            Assert.Equivalent(
                schemaSymbol,
                new Symbol() {
                    Name = nameof(schema),
                    Node = schema,
                    Visibility = Symbol.SymbolVisibility.Local
                }
            );
        }
    }
}