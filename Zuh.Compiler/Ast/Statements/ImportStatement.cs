namespace Zuh.Compiler.Ast {
    public record ImportStatement : Statement {
        public required StringLiteral Module { get; init; }
    }
}