using Zuh.Compiler.Ast;
using Zuh.Compiler.Diagnostics;
using Zuh.Compiler.Semantics.Diagnostics;
using Zuh.Compiler.Semantics.Symbols;
using Zuh.Compiler.Semantics.Visitors;

namespace Zuh.Compiler.Semantics.Analyzers {
    /// <summary>
    /// analyzes a single unit of compilation (a <see cref="ZuhFile"/>) for semantic data.
    /// </summary>
    public class UnitAnalyzer {
        /// <summary>
        /// parent compilation analyzer.
        /// </summary>
        /// <remarks>
        /// required in order to access cached module imports.
        /// </remarks>
        public required CompilationAnalyzer CompilationAnalyzer { get; init; }
        
        /// <summary>
        /// ast representation of the unit to analyze.
        /// </summary>
        public required ZuhFile UnitAst { get; init; }
        
        /// <summary>
        /// name of the unit to analyze.
        /// </summary>
        public required string UnitId { get; init; }

        public ScopeTracker ScopeTracker { get; init; } = new();
        public SymbolTracker SymbolTracker { get; init; } = new();
        
        public DiagnosticCollector Diagnostics { get; private init; } = [];

        /// <summary>
        /// analyzes the single unit.
        /// </summary>
        public void Analyze() {
            // scope creation
            var scopeCreatorVisitor = new ScopeCreatorVisitor() {
                ScopeTracker = ScopeTracker
            };
            
            scopeCreatorVisitor.Visit(UnitAst);
            
            // symbol declaration
            var symbolDeclarationVisitor = new SymbolDeclarationVisitor() {
                ScopeTracker = ScopeTracker
            };
            
            symbolDeclarationVisitor.Visit(UnitAst);

            foreach(var statement in UnitAst.RootStatements) {
                if(statement is not ImportStatement importStatement)
                    continue;
                
                handleImport(importStatement);
            }
            
            // identifier resolution
            var identifierResolverVisitor = new IdentifierResolverVisitor() {
                ScopeTracker = ScopeTracker,
                SymbolTracker = SymbolTracker
            };
            
            identifierResolverVisitor.Visit(UnitAst);
        }

        private void handleImport(ImportStatement importStatement) {
            var importedModuleName = importStatement.Module.Value;
            var resolution = CompilationAnalyzer.ImportResolver.ResolveImport(UnitId, importedModuleName);

            if(resolution is { Success: false }) {
                Diagnostics.Add(new ModuleResolutionError() {
                    ModuleName = importedModuleName,
                    Location = importStatement.SourceSpan
                });

                return;
            }
            
            var importedFileAnalyzer = CompilationAnalyzer.ImportUnitAnalyzer(resolution);

            // transfer all exported things
            foreach(var (key, symbol) in importedFileAnalyzer.ScopeTracker.NodeToPersonalScope[importedFileAnalyzer.UnitAst]) {
                if(symbol is not ExportableSymbol { IsExport: true } exportableSymbol)
                    continue;

                ScopeTracker.NodeToPersonalScope[UnitAst].Declare(exportableSymbol with {
                    IsExport = false
                });
            }
        }
    }
}