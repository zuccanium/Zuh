namespace Zuh.Compiler.Ast {
    public record KeysEntry : ZuhNode {
        public required Identifier Identifier { get; init; }
    }
}