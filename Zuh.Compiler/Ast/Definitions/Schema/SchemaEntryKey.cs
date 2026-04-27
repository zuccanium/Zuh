namespace Zuh.Compiler.Ast {
    public abstract record SchemaEntryKey : ZuhNode {
        public bool IsOptional { get; init; } = false;
    }
}