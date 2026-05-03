namespace Zuh.Compiler.Ast {
    public record ExpressionKey : DynamicKey {
        public required Expression Expression { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            yield return Expression;
        }
    }
}