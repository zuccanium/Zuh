using System.Collections.Immutable;

namespace Zuh.Compiler.Ast {
    public record Schema : ZuhNode {
        public required ImmutableArray<SchemaEntry> Entries { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            foreach(var entry in Entries)
                yield return entry;
        }
    }
}