namespace Zuh.Compiler.Ast {
    public abstract record Key : ZuhNode {
        public bool IsOptional { get; init; } = false;
    }
}