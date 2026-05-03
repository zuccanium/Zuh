using System.Collections.Immutable;

namespace Zuh.Compiler.Ast {
    public record Function : ZuhNode, IHasScope {
        public required ImmutableArray<FunctionParameter> Parameters { get; init; }
        public required Expression Expression { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            foreach(var parameter in Parameters)
                yield return parameter;

            yield return Expression;
        }
    }
}