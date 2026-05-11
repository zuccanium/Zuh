using System.Collections.Immutable;

namespace Zuh.Compiler.Ast {
    public record SumEntry : ZuhNode, IDocumentationHolder {
        public ImmutableArray<DocumentationLine>? DocumentationLines { get; init; }
        
        public required Key Key { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            yield return Key;
        }
    }
}