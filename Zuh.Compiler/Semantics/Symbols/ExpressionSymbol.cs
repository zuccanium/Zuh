using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Semantics.Symbols {
    public record ExpressionSymbol : ExportableSymbol {
        public required Expression Expression { get; init; }
    }
}