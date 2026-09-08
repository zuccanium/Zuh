using Zuh.Compiler.Ast;
using Zuh.Compiler.Semantics.Types;

namespace Zuh.Compiler.Semantics.Symbols {
    public abstract class Symbol {
        public required string Name { get; init; }
        public required string UnitId { get; init; }
        public ZuhType? Type { get; set; }
        
        public abstract ZuhNode Node { get; }
    }
}