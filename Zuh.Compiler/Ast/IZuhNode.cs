namespace Zuh.Compiler.Ast {
    public interface IZuhNode {
        public SourceSpan? SourceSpan { get; init; }

        public IEnumerator<IZuhNode> GetChildrenEnumerator();
    }
}