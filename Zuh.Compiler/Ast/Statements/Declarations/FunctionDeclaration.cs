namespace Zuh.Compiler.Ast {
    public record FunctionDeclaration : Declaration {
        public required Identifier Name { get; init; }
        public required Function Function { get; init; }
    }
}