using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Semantics.Symbols {
    public record SchemaSymbol : ExportableSymbol {
        public required Schema Schema { get; init; }
    }
}