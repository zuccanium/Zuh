namespace Zuh.Compiler.Ast {
    public record KeysExpression : Expression {
        public required Keys Keys { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            yield return Keys;
        }
    }
}