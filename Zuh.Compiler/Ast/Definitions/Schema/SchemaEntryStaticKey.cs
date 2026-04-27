namespace Zuh.Compiler.Ast {
    public record SchemaEntryStaticKey : SchemaEntryKey {
        public required Identifier Key { get; init; }
    }
}