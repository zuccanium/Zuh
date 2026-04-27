using System.Collections.Immutable;

namespace Zuh.Compiler.Ast {
    public record Schema : ZuhNode {
        public required ImmutableArray<SchemaEntry> Entries { get; init; }
    }
}