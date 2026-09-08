using System.Collections;
using Zuh.Compiler.Semantics.Symbols;

namespace Zuh.Compiler.Semantics {
    /// <summary>
    /// keeps track of all symbols in a scope and handles resolution between a scope and its ancestors.
    /// </summary>
    public class Scope : IEnumerable<KeyValuePair<string, Symbol>> {
        public Dictionary<string, Symbol> Symbols { get; init; } = [];
        public Scope? Parent { get; init; }

        /// <summary>
        /// declares a symbol in the scope.
        /// </summary>
        /// <param name="entry">the symbol to declare.</param>
        /// <returns>true if it worked; false if the symbol name already existed.</returns>
        public bool Declare(Symbol entry)
            => Symbols.TryAdd(entry.Name, entry);

        /// <summary>
        /// attempts to resolve a symbol with a given name. this will call itself on the parent scope if it cant find it.
        /// </summary>
        /// <param name="name">symbol name.</param>
        /// <returns>the symbol optionally.</returns>
        public Symbol? Resolve(string name) {
            if(Symbols.TryGetValue(name, out var symbol))
                return symbol;

            if(Parent is { } parent)
                return parent.Resolve(name);

            return null!;
        }

        public IEnumerator<KeyValuePair<string, Symbol>> GetEnumerator()
            => Symbols.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}