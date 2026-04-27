using System.Collections.Immutable;

namespace Zuh.Compiler.Ast {
    public record SchemaEntry : ZuhNode, ITriviaHolder {
        public ImmutableArray<string>? TriviaLines { get; init; }

        public required SchemaEntryKey Key { get; init; }
        public Expression? Value { get; init; }
    }
}