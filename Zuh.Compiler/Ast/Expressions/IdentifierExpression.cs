namespace Zuh.Compiler.Ast {
    public record IdentifierExpression : Expression {
        public required Identifier Identifier { get; init; }
    }
}