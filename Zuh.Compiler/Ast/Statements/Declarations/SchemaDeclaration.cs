namespace Zuh.Compiler.Ast {
    public record SchemaDeclaration : Declaration {
        public required Identifier Name { get; init; }
        public required Schema Schema { get; init; }
    }
}