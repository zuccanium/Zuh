using System.Collections.Immutable;

namespace Zuh.Compiler.Ast {
    public record ZuhFile : ZuhNode {
        public required ImmutableArray<Statement> RootStatements { get; init; }
    }
}