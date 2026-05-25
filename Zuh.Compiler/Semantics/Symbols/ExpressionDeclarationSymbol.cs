using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Semantics.Symbols {
    public class ExpressionDeclarationSymbol : ExportableSymbol {
        public required ExpressionDeclaration ExpressionDeclaration { get; init; }

        public override ZuhNode Node
            => ExpressionDeclaration;
    }
}