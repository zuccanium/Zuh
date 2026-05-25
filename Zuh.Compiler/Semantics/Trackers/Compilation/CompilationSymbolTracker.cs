using System.Collections.Immutable;
using Zuh.Compiler.Semantics.Symbols;

namespace Zuh.Compiler.Semantics.Trackers.Compilation {
    /// <summary>
    /// tracks all symbols in a compilation.
    /// </summary>
    public class CompilationSymbolTracker {
        /// <summary>
        /// encapsulates a circular dependency between symbols
        /// </summary>
        /// <param name="symbols">all the symbols involved in the circle (e.g. a -> b -> c).</param>
        public record CircularDependency(params ImmutableArray<Symbol> symbols) {
            private const string Joiner = " -> ";
            
            private readonly ImmutableArray<Symbol> symbols = symbols;

            // maybe make noncommutative?
            public override int GetHashCode()
                => symbols
                    .Select(symbol => symbol.GetHashCode())
                    .Aggregate((x, y) => x ^ y);

            public override string ToString()
                => string.Join(Joiner, from symbol in symbols select symbol.Name) + Joiner + symbols.First();
        }
        
        /// <summary>
        /// map of symbol -> the symbols referenced by the symbol.
        /// </summary>
        public Dictionary<Symbol, HashSet<Symbol>> SymbolToDependencies { get; set; } = [];
        
        /// <summary>
        /// map of symbol -> the circular dependencies that the node is involved in.
        /// </summary>
        public Dictionary<Symbol, HashSet<CircularDependency>> SymbolToCircularDependencies { get; private init; } = [];
        
        /// <summary>
        /// traverses <see cref="SymbolToDependencies"/> to populate <see cref="CircularDependencies"/> and <see cref="CircularSymbols"/>.
        /// </summary>
        public void ResolveCircularDependencies() {
            // why doesnt c# have a builtin ordered set
            // this is nasty
            void recur(Symbol startSymbol, Symbol currentSymbol, HashSet<Symbol> seen, ImmutableArray<Symbol> list) {
                if(currentSymbol == startSymbol && seen.Count > 0) {
                    if(!SymbolToCircularDependencies.TryGetValue(startSymbol, out var set))
                        SymbolToCircularDependencies[startSymbol] = set = [];
                    
                    set.Add(new CircularDependency([..list]));

                    return;
                }

                if(seen.Contains(currentSymbol))
                    return;
                
                var currentSymbolDependencies = SymbolToDependencies[currentSymbol];

                foreach(var dependency in currentSymbolDependencies)
                    recur(startSymbol, dependency, [..seen, currentSymbol], [..list, currentSymbol]);
            }

            foreach(var (symbol, _) in SymbolToDependencies)
                recur(symbol, symbol, [], []);
        }
    }
}