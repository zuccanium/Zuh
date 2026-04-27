using System.Collections.Immutable;

namespace Zuh.Compiler.Ast {
    public record Function : ZuhNode {
        public required ImmutableArray<FunctionParameter> Parameters { get; init; }
        public required Expression Expression { get; init; }
    }
}