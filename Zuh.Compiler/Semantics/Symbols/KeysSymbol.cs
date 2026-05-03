using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Semantics.Symbols {
    public record KeysSymbol : Symbol {
        public required Keys Keys { get; init; }
    }
}