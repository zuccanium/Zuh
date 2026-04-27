namespace Zuh.Compiler.Ast {
    public abstract record BinaryExpression : Expression {
        public required Expression Right { get; init; }
        public required Expression Left { get; init; }
    }
}