using Zuh.Compiler.Ast;
using Zuh.Compiler.Diagnostics;
using Zuh.Compiler.Semantics.Diagnostics;
using Zuh.Compiler.Semantics.Symbols;
using Zuh.Compiler.Semantics.Visitors;

namespace Zuh.Compiler.Semantics.Analyzers {
    public class UnitAnalyzer : Analyzer {
        public required CompilationAnalyzer CompilationAnalyzer { get; init; }
        
        public required ZuhFile File { get; init; }
        public required string UnitId { get; init; }

        public ScopeTracker ScopeTracker { get; init; } = new();
        public SymbolTracker SymbolTracker { get; init; } = new();

        public override void Analyze() {
            // scope creation
            var scopeCreatorVisitor = new ScopeCreatorVisitor() {
                ScopeTracker = ScopeTracker
            };
            
            scopeCreatorVisitor.Visit(File);
            
            // symbol declaration
            var symbolDeclarationVisitor = new SymbolDeclarationVisitor() {
                ScopeTracker = ScopeTracker
            };
            
            symbolDeclarationVisitor.Visit(File);

            foreach(var statement in File.RootStatements) {
                if(statement is not ImportStatement importStatement)
                    continue;
                
                handleImport(importStatement);
            }
            
            // identifier resolution
            var identifierResolverVisitor = new IdentifierResolverVisitor() {
                ScopeTracker = ScopeTracker,
                SymbolTracker = SymbolTracker
            };
            
            identifierResolverVisitor.Visit(File);
        }

        private void handleImport(ImportStatement importStatement) {
            var result = CompilationAnalyzer.ImportUnitAnalyzer(UnitId, importStatement.Module.Value);

            if(result is { Diagnostic: { } diagnostic })
                return;

            var analyzer = result.Value!;
            
            // transfer all exported things
            foreach(var (key, symbol) in analyzer.ScopeTracker.NodeToPersonalScope[analyzer.File]) {
                if(symbol.Visibility != Symbol.SymbolVisibility.Exported)
                    continue;

                ScopeTracker.NodeToPersonalScope[File].Declare(symbol with {
                    Visibility = Symbol.SymbolVisibility.Local
                });
            }
        }
    }
}