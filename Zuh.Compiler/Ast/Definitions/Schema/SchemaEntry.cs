using System.Collections.Immutable;

namespace Zuh.Compiler.Ast {
    public record SchemaEntry : ZuhNode, ITriviaHolder {
        public ImmutableArray<string>? TriviaLines { get; init; }

        public required Key Key { get; init; }
        public Expression? Value { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            yield return Key;
            
            if(Value is {} value)
                yield return value;
        }
    }
}