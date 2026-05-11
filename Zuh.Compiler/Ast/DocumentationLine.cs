namespace Zuh.Compiler.Ast {
    public record DocumentationLine : ZuhNode {
        public required string Value { get; init; }
        
        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            yield break;
        }
    }
}