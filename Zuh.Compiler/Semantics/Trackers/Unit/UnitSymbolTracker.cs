using Zuh.Compiler.Ast;
using Zuh.Compiler.Semantics.Symbols;

namespace Zuh.Compiler.Semantics.Trackers.Unit {
    /// <summary>
    /// keeps track of symbol information associated with nodes in a <see cref="ZuhFile"/>.
    /// </summary>
    public class UnitSymbolTracker {
        /// <summary>
        /// map of nodes that have a symbol -> the symbol of the node
        /// </summary>
        public Dictionary<ZuhNode, Symbol> NodeToPersonalSymbol { get; set; } = [];
        
        /// <summary>
        /// map of identifiers -> the symbol that the identifier identifies
        /// </summary>
        public Dictionary<Identifier, Symbol> IdentifierToSymbol { get; set; } = [];
    }
}