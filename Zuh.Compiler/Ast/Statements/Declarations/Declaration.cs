using System.Collections.Immutable;
using Zuh.Compiler.Semantics.Visitors;

namespace Zuh.Compiler.Ast {
    public abstract record Declaration : Statement, IExistsInScope, IDocumentationHolder {
        public ImmutableArray<DocumentationLine>? DocumentationLines { get; init; }
        
        public required Label Name { get; init; }
        public bool IsExport { get; init; }
    }
}