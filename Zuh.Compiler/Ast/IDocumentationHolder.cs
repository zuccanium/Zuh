using System.Collections.Immutable;

namespace Zuh.Compiler.Ast {
    public interface IDocumentationHolder {
        public ImmutableArray<DocumentationLine>? DocumentationLines { get; init; }
    }
}