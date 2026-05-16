using Zuh.Compiler.Ast;
using Zuh.Compiler.Diagnostics;
using Zuh.Compiler.Parsing;
using Zuh.Compiler.Semantics.Diagnostics;
using Zuh.Compiler.Utils;

namespace Zuh.Compiler.Semantics.Analyzers {
    /// <summary>
    /// analyzes a multiple units of compilation (<see cref="ZuhFile"/>) for semantic data.
    /// </summary>
    public class CompilationAnalyzer {
        /// <summary>
        /// the thing used to handle imports.
        /// </summary>
        public required IImportResolver ImportResolver { get; init; }
        
        /// <summary>
        /// map of unit id -> file ast.
        /// this is where you input files to be analyzed.
        /// </summary>
        public required Dictionary<string, ZuhFile> UnitAsts { get; init; }

        /// <summary>
        /// map of unit id -> unit analyzer.
        /// </summary>
        public Dictionary<string, UnitAnalyzer> UnitAnalyzers { get; private init; } = [];

        /// <summary>
        /// aggregation of the diagnostics of each unit.
        /// </summary>
        public DiagnosticCollector Diagnostics
            => DiagnosticCollector.Merge(from analyzer in UnitAnalyzers select Diagnostics);
        
        /// <summary>
        /// analyzes the files provided in <see cref="UnitAsts"/>.
        /// </summary>
        public void Analyze() {
            foreach(var (unitId, unit) in UnitAsts) {
                var unitAnalyzer = new UnitAnalyzer() {
                    UnitAst = unit,
                    UnitId = unitId,
                    CompilationAnalyzer = this
                };
                
                UnitAnalyzers[unitId] = unitAnalyzer;
                
                unitAnalyzer.Analyze();
            }
        }
        
        /// <summary>
        /// allows for <see cref="UnitAnalyzer"/>s to interact with each other through imports.
        /// </summary>
        /// <param name="importResolution">an import resolution <b>that is expected to be a success</b>.</param>
        /// <returns>the unit analyzer associated with the provided resolution.</returns>
        public UnitAnalyzer ImportUnitAnalyzer(IImportResolution importResolution) {
            if(UnitAnalyzers.TryGetValue(importResolution.Id!, out var cachedAnalyzer))
                return cachedAnalyzer;

            var file = importResolution.FetchFile(out var diagnostics);
            
            var analyzer = new UnitAnalyzer() {
                CompilationAnalyzer = this,
                UnitAst = file,
                UnitId = importResolution.Id!
            };
            
            UnitAnalyzers[importResolution.Id!] = analyzer;
            
            analyzer.Analyze();

            return analyzer;
        }
    }
}