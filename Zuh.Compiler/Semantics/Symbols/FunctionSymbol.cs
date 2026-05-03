using System.Collections.Immutable;
using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Semantics.Symbols {
    public record FunctionSymbol : Symbol {
        public required Function Function { get; init; }
        public required ImmutableArray<FunctionParameterSymbol> Parameters { get; init; }
    }
}