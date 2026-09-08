using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Semantics.Trackers.Unit {
    /// <summary>
    /// keeps track of scope information in a <see cref="ZuhFile"/>.
    /// </summary>
    public class UnitScopeTracker {
        /// <summary>
        /// map of nodes that own a scope -> the scope the node owns.
        /// </summary>
        public Dictionary<IHasScope, Scope> NodeToPersonalScope { get; set; } = [];
        
        /// <summary>
        /// map of nodes that exist in a scope -> the scope the node exists in.
        /// </summary>
        public Dictionary<IExistsInScope, Scope> NodeToEnclosingScope { get; set; } = [];
    }
}