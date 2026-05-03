using System.Collections.Immutable;

namespace Zuh.Compiler.Ast {
    public record Keys : ZuhNode {
        public required ImmutableArray<KeysEntry> Entries { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            foreach(var entry in Entries)
                yield return entry;
        }
    }
}