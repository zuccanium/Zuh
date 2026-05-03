using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Semantics {
    /// <summary>
    /// tracks the symbols associated with identifiers
    /// </summary>
    public class SymbolTracker {
        public Dictionary<Identifier, Symbol> Symbols { get; set; } = [];
    }
}