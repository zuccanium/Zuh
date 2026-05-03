namespace Zuh.Compiler.Ast {
    public record ArrayExpression : Expression {
        public required Expression Expression { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            yield return Expression;
        }
    }
}