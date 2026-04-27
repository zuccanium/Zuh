namespace Zuh.Compiler.Ast {
    public record KeysExpression : Expression {
        public required Keys Keys { get; init; }
    }
}