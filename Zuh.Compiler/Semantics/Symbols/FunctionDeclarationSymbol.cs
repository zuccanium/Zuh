using System.Collections.Immutable;
using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Semantics.Symbols {
    public class FunctionDeclarationSymbol : ExportableSymbol {
        public required FunctionDeclaration FunctionDeclaration { get; init; }
        public required ImmutableArray<FunctionParameterSymbol> Parameters { get; init; }

        public override ZuhNode Node
            => FunctionDeclaration;
    }
}