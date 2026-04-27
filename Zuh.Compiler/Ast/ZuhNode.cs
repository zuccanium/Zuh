namespace Zuh.Compiler.Ast {
    public abstract record ZuhNode {
        public SourceSpan? SourceSpan { get; init; }
    }
}