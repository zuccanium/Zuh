using System.Collections.Immutable;

namespace Zuh.Compiler.Ast {
    public record ZuhFile : ZuhNode, IHasScope {
        public required ImmutableArray<Statement> RootStatements { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            foreach(var statement in RootStatements)
                yield return statement;
        }
    }
}