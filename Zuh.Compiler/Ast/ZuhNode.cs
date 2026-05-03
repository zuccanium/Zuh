namespace Zuh.Compiler.Ast {
    public abstract record ZuhNode : IZuhNode {
        public SourceSpan? SourceSpan { get; init; }

        public abstract IEnumerator<IZuhNode> GetChildrenEnumerator();
    }
}