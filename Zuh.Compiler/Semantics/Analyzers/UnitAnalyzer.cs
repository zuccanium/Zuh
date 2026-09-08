using Zuh.Compiler.Ast;
using Zuh.Compiler.Diagnostics;
using Zuh.Compiler.Semantics.Diagnostics;
using Zuh.Compiler.Semantics.Symbols;
using Zuh.Compiler.Semantics.Trackers.Compilation;
using Zuh.Compiler.Semantics.Trackers.Unit;
using Zuh.Compiler.Semantics.Visitors;

namespace Zuh.Compiler.Semantics.Analyzers {
    /// <summary>
    /// analyzes a single unit of compilation (a <see cref="ZuhFile"/>) for semantic data.
    /// </summary>
    public class UnitAnalyzer {
        /// <summary>
        /// ast representation of the unit to analyze.
        /// </summary>
        public required ZuhFile UnitAst { get; init; }
        
        /// <summary>
        /// name of the unit to analyze.
        /// </summary>
        public required string UnitId { get; init; }

        public UnitScopeTracker UnitScopeTracker { get; init; } = new();
        public UnitSymbolTracker UnitSymbolTracker { get; init; } = new();
        public UnitTypeTracker UnitTypeTracker { get; init; } = new();
        
        public DiagnosticCollector Diagnostics { get; private init; } = [];

        /// <summary>
        /// everything that can be done in full isolation from other units.
        /// </summary>
        public void CreateScopesAndSymbols() {
            // scope creation
            var scopeCreatorVisitor = new ScopeCreatorVisitor() {
                UnitScopeTracker = UnitScopeTracker
            };

            scopeCreatorVisitor.Visit(UnitAst);

            // symbol declaration
            var symbolDeclarationVisitor = new SymbolDeclarationVisitor() {
                UnitScopeTracker = UnitScopeTracker,
                UnitSymbolTracker = UnitSymbolTracker,
                Diagnostics = Diagnostics,
                UnitId = UnitId
            };

            symbolDeclarationVisitor.Visit(UnitAst);
        }

        /// <summary>
        /// add all imports content to the root scope.
        /// </summary>
        /// <param name="compilationAnalyzer">used to actually fetch the imports.</param>
        public void HandleImports(CompilationAnalyzer compilationAnalyzer) {
            foreach(var statement in UnitAst.RootStatements) {
                if (statement is not ImportStatement importStatement)
                    continue;

                handleImport(importStatement, compilationAnalyzer);
            }
        }

        /// <summary>
        /// figure out what identifiers identify and what symbols depend on.
        /// </summary>
        public void AnalyzeSymbolReferences(CompilationSymbolTracker compilationSymbolTracker) {
            // identifier resolution
            var identifierResolverVisitor = new IdentifierResolverVisitor() {
                UnitScopeTracker = UnitScopeTracker,
                Diagnostics = Diagnostics,
                UnitSymbolTracker = UnitSymbolTracker
            };

            identifierResolverVisitor.Visit(UnitAst);
            
            // dependency finding
            var symbolDependencyFinderVisitor = new SymbolDependencyFinderVisitor() {
                UnitSymbolTracker = UnitSymbolTracker,
                CompilationSymbolTracker = compilationSymbolTracker
            };
            
            symbolDependencyFinderVisitor.Visit(UnitAst);
        }

        private void handleImport(ImportStatement importStatement, CompilationAnalyzer compilationAnalyzer) {
            var importedModuleName = importStatement.Module.Value;
            var resolution = compilationAnalyzer.ImportResolver.ResolveImport(UnitId, importedModuleName);

            if(resolution is { Success: false }) {
                Diagnostics.Add(new ModuleResolutionError() {
                    ModuleName = importedModuleName,
                    Location = importStatement.SourceSpan
                });

                return;
            }

            var importedUnitId = resolution.Id;
            var importedFileAnalyzer = compilationAnalyzer.ImportUnitAnalyzer(resolution);

            // transfer all exported things
            foreach(var (symbolName, symbol) in importedFileAnalyzer.UnitScopeTracker.NodeToPersonalScope[importedFileAnalyzer.UnitAst]) {
                if(symbol is not ExportableSymbol { IsExport: true } exportableSymbol)
                    continue;

                // exports can only be transferred between units if they come from their original unit
                if(exportableSymbol.UnitId != importedUnitId)
                    continue;

                if(UnitScopeTracker.NodeToPersonalScope[UnitAst].Declare(exportableSymbol))
                    continue;
                
                Diagnostics.Add(new DeclarationError() {
                    Location = importStatement.SourceSpan,
                    DeclarationName = symbolName
                });
            }
        }
    }
}