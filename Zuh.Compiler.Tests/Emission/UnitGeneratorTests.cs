using Zuh.Compiler.Ast;
using Zuh.Compiler.Emission;
using Zuh.Compiler.Emission.Nodes;
using Zuh.Compiler.Semantics;
using Zuh.Compiler.Semantics.Analyzers;

namespace Zuh.Compiler.Tests.Emission {
    public class UnitGeneratorTests {
        [Fact]
        public void Generate_BasicSchema_Works() {
            const string keyName = "key";
            
            var schema = new SchemaDeclaration() {
                IsExport = true,
                Name = new Label() {
                    Value = "schema"
                },
                Schema = new Schema() {
                    Entries = [
                        new SchemaEntry() {
                            Key = new SchemaEntryStaticKey() {
                                Name = new Label() {
                                    Value = keyName
                                }
                            }
                        }
                    ]
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

            var symbolTracker = new SymbolTracker();

            var analyzer = new UnitAnalyzer() {
                CompilationAnalyzer = null!,
                File = file,
                UnitId = "main.zuh",
                ScopeTracker = scopeTracker,
                SymbolTracker = symbolTracker,
            };

            var generator = new UnitGenerator() {
                Analyzer = analyzer
            };
            
            generator.Generate();
            
            Assert.Equivalent(generator.Root, new MappingNode() {
                [nameof(schema)] = new MappingNode.Value() {
                    Node = new MappingNode() {
                        [keyName] = new MappingNode.Value() {
                            Node = new ScalarNode()
                        }
                    }
                }
            });
        }
    }
}