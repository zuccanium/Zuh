namespace Zuh.Compiler.Ast {
    public record Identifier : ZuhNode, IExistsInScope {
        public required string Value { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            yield break;
        }
    }
}