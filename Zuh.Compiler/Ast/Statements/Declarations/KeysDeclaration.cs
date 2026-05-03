namespace Zuh.Compiler.Ast {
    public record KeysDeclaration : Declaration {
        public required Keys Keys { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            yield return Keys;
        }
    }
}