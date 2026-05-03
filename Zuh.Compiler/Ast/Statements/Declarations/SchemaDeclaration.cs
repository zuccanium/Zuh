namespace Zuh.Compiler.Ast {
    public record SchemaDeclaration : Declaration {
        public required Schema Schema { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            yield return Schema;
        }
    }
}