namespace Zuh.Compiler.Ast {
    public record SchemaExpression : Expression {
        public required Schema Schema { get; init; }
    }
}