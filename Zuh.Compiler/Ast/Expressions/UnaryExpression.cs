namespace Zuh.Compiler.Ast {
    public abstract record UnaryExpression : Expression {
        public required Expression Expression { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            yield return Expression;
        }
    }
}