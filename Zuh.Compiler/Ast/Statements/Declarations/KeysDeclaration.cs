namespace Zuh.Compiler.Ast {
    public record KeysDeclaration : Declaration {
        public required Identifier Name { get; init; }
        public required Keys Keys { get; init; }
    }
}