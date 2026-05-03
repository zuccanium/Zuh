namespace Zuh.Compiler.Ast {
    public record SumExpression : Expression {
        public required Sum Sum { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            yield return Sum;
        }
    }
}