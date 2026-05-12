using System.Collections.Immutable;

namespace Zuh.Compiler.Ast {
    public record ExpressionDeclaration : Declaration, IDocumentationHolder {
        public ImmutableArray<DocumentationLine>? DocumentationLines { get; init; }
        public required Expression Expression { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            yield return Expression;
        }
    }
}