using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Semantics.Symbols {
    public record KeysSymbol : ExportableSymbol {
        public required Keys Keys { get; init; }
    }
}