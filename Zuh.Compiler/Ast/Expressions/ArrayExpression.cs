namespace Zuh.Compiler.Ast {
    public record ArrayExpression : Expression {
        public required Expression Expression { get; init; }
    }
}