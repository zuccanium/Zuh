using Zuh.Compiler.Generation.Nodes;
using Zuh.Compiler.Semantics.Analyzers;

namespace Zuh.Compiler.Generation {
    /// <summary>
    /// thing that generates root mapping nodes for each unit in a compilation.
    /// </summary>
    public class Generator {
        /// <summary>
        /// the compilation analyzer to reference for semantic data.
        /// </summary>
        public required CompilationAnalyzer Analyzer { get; init; }

        public Dictionary<string, MappingNode> Generate(IEnumerable<string> unitIds) {
            var dictionary = new Dictionary<string, MappingNode>();
            
            foreach(var unitId in unitIds) {
                if(!Analyzer.UnitAnalyzers.TryGetValue(unitId, out var unitAnalyzer))
                    throw new InvalidOperationException($"{unitId} doesnt have an analyzer!");

                var unitGenerator = new UnitGenerator() {
                    Analyzer = unitAnalyzer
                };

                dictionary[unitId] = unitGenerator.Generate();
            }

            return dictionary;
        }
    }
}