using System.Collections.Immutable;

namespace Zuh.Compiler.Ast {
    public record Keys : ZuhNode {
        public required ImmutableArray<KeysEntry> Entries { get; init; }
    }
}