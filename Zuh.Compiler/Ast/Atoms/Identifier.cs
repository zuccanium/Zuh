namespace Zuh.Compiler.Ast {
    public record Identifier : ZuhNode {
        public required string Value { get; init; }
    }
}