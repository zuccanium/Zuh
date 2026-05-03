namespace Zuh.Compiler.Ast {
    public record FunctionDeclaration : Declaration {
        public required Function Function { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            yield return Function;
        }
    }
}