using System.Collections.Immutable;

namespace Zuh.Compiler.Ast {
    public record SumEntry : ZuhNode, ITriviaHolder {
        public ImmutableArray<string>? TriviaLines { get; init; }
        
        public required Key Key { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            yield return Key;
        }
    }
}