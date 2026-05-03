namespace Zuh.Compiler.Ast {
    public record ExpressionDeclaration : Declaration {
        public required Expression Expression { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            yield return Expression;
        }
    }
}