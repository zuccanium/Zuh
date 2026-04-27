namespace Zuh.Compiler.Ast {
    public record SchemaEntryExpressionKey : SchemaEntryDynamicKey {
        public required Expression Expression { get; init; }
    }
}