using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Semantics.Symbols {
    public record SchemaSymbol : Symbol {
        public required Schema Schema { get; init; }
    }
}