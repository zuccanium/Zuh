using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Semantics {
    /// <summary>
    /// keeps track of scope information in a <see cref="ZuhFile"/>
    /// </summary>
    public class ScopeTracker {
        public Dictionary<IHasScope, Scope> NodeToPersonalScope { get; set; } = [];
        public Dictionary<IExistsInScope, Scope> NodeToEnclosingScope { get; set; } = [];
    }
}