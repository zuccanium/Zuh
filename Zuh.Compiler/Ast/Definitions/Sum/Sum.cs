using System.Collections.Immutable;

namespace Zuh.Compiler.Ast {
    public record Sum : ZuhNode {
        public required ImmutableArray<SumEntry> Entries { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            foreach(var entry in Entries)
                yield return entry;
        }
    }
}