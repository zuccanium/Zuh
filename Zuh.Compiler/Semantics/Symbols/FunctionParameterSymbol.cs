using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Semantics.Symbols {
    public record FunctionParameterSymbol : Symbol {
        public required FunctionParameter FunctionParameter { get; init; }
    }
}