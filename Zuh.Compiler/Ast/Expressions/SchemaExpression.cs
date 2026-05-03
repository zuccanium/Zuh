namespace Zuh.Compiler.Ast {
    public record SchemaExpression : Expression {
        public required Schema Schema { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            yield return Schema;
        }
    }
}