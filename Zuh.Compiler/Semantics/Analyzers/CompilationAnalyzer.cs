using Zuh.Compiler.Ast;
using Zuh.Compiler.Diagnostics;
using Zuh.Compiler.Parsing;
using Zuh.Compiler.Semantics.Diagnostics;
using Zuh.Compiler.Semantics.Symbols;
using Zuh.Compiler.Semantics.Trackers.Compilation;
using Zuh.Compiler.Semantics.Visitors;
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

        public CompilationSymbolTracker CompilationSymbolTracker { get; private init; } = new();

        /// <summary>
        /// analyzes the files provided in <see cref="UnitAsts"/>.
        /// </summary>
        public void Analyze() {
            // okay i need to write a description of this entire process to keep myself sane
            // - create all individual unit analyzers for the specified units
            // - do all isolated analysis on specified units
            //   - for each unit analyzer
            //     - create and track scopes
            //       - go through the syntax tree
            //         - for any node marked as IHasScope, create a scope
            //           - tracked in ScopeTracker.NodeToPersonalScope
            //         - for any node marked as IExistsInScope, keep track of which scope that node exists in
            //           - tracked in ScopeTracker.NodeToEnclosingScope
            //     - create and track symbols
            //       - go through the syntax tree
            //         - on nodes that have associated symbols, create those symbols
            //           - declare those symbols in the scopes of their respective nodes
            //           - keep track of which node owns which symbol
            //             - tracked in SymbolTracker.NodeToPersonalSymbol
            //           - examples
            //             - ZuhFile gets symbols for all its declarations
            //             - Function gets symbols for all its parameters
            // - handle imports
            //   - for each unit analyzer
            //     - for each import in that unit analyzer
            //       - resolve the import
            //         - if unsuccessful, create a diagnostic and move on
            //       - get the unit analyzer with all the isolated analysis done
            //         - this supports caching
            //       - for each exported symbol in the imported unit analyzer
            //         - add that symbol to the unit scope of the file trying to import it
            // - do all semi isolated analysis on all units
            //   - as in any traversal can be confined to a single unit
            //   - for each unit analyzer
            //     - resolve identifiers
            //       - go through the syntax tree
            //         - for any identifier node
            //           - get its enclosing scope with ScopeTracker.NodeToEnclosingScope
            //           - attempt to resolve the value of the identifier in that scope
            //             - tracked in SymbolTracker.IdentifierToSymbol
            //     - find all symbol dependencies
            //       - a symbol dependency is just a situation where a symbol relies on another symbol for any reason
            //       - go through the syntax tree
            //         - for any declaration node, obtain the symbols of all identifiers within it
            //           - tracked in SymbolTracker.SymbolToDependencies
            // - do all unisolated analysis
            //   - it is at this point that the unit analyzers have blended together
            //   - they can no longer be analyzed as units
            //   - check for circular dependencies in the newly created dependency graph
            //     - mark symbols involved as evil and ignored for later processing
            //   - dfs through the dependency graph
            //     - if the symbol already has a type, continue
            //     - recur into branches
            //     - tell the type resolver visitor to resolve the symbol's node
            //       - tracked in TypeTracker.ExpressionToType
            
            foreach(var (unitId, unit) in UnitAsts)
                UnitAnalyzers[unitId] = new UnitAnalyzer() {
                    UnitAst = unit,
                    UnitId = unitId
                };
            
            // gills 👇

            foreach(var (_, unitAnalyzer) in UnitAnalyzers)
                unitAnalyzer.CreateScopesAndSymbols();
            
            foreach(var (unitId, _) in UnitAsts)
                UnitAnalyzers[unitId].HandleImports(this);
            
            foreach(var (_, unitAnalyzer) in UnitAnalyzers)
                unitAnalyzer.AnalyzeSymbolReferences(CompilationSymbolTracker);
            
            CompilationSymbolTracker.ResolveCircularDependencies();

            foreach(var (symbol, circularDependencies) in CompilationSymbolTracker.SymbolToCircularDependencies)
                foreach(var circularDependency in circularDependencies)
                    UnitAnalyzers[symbol.UnitId].Diagnostics.Add(new CircularDependencyError() {
                        Name = symbol.Name,
                        CircularDependency = circularDependency,
                        Location = symbol.Node.SourceSpan
                    });
            
            var unitTypeResolverVisitors = new Dictionary<string, TypeResolverVisitor>();

            foreach(var (unitId, unitAnalyzer) in UnitAnalyzers)
                unitTypeResolverVisitors[unitId] = new TypeResolverVisitor() {
                    UnitTypeTracker = unitAnalyzer.UnitTypeTracker,
                    UnitSymbolTracker = unitAnalyzer.UnitSymbolTracker,
                    Diagnostics = unitAnalyzer.Diagnostics
                };

            // dfs application holy moly
            // returns true if the type was successfully resolved
            bool resolveRecur(Symbol symbol) {
                if(symbol.Type is not null)
                    return true;

                if(CompilationSymbolTracker.SymbolToCircularDependencies.ContainsKey(symbol))
                    return false;
                
                var dependencies = CompilationSymbolTracker.SymbolToDependencies[symbol];
                var allResolved = dependencies.All(resolveRecur);

                // if a single dependency cant be resolved, this cant be either
                if(!allResolved)
                    return false;
                
                var resolverVisitor = unitTypeResolverVisitors[symbol.UnitId];
                
                resolverVisitor.Visit(symbol.Node);

                return true;
            }
            
            foreach(var (symbol, _) in CompilationSymbolTracker.SymbolToDependencies)
                resolveRecur(symbol);
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
                UnitAst = file,
                UnitId = importResolution.Id!
            };
            
            UnitAnalyzers[importResolution.Id!] = analyzer;
            
            analyzer.CreateScopesAndSymbols();
            analyzer.HandleImports(this);

            return analyzer;
        }
    }
}