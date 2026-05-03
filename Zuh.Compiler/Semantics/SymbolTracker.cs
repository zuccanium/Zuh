using Zuh.Compiler.Ast;
using Zuh.Compiler.Semantics.Symbols;

namespace Zuh.Compiler.Semantics {
    /// <summary>
    /// tracks the symbols associated with identifiers.
    /// </summary>
    public class SymbolTracker {
        public Dictionary<Identifier, Symbol> Symbols { get; set; } = [];
    }
}