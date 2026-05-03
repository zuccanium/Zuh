namespace Zuh.Compiler.Ast {
    public record SchemaEntryExpressionKey : SchemaEntryDynamicKey {
        public required Expression Expression { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            yield return Expression;
        }
    }
}