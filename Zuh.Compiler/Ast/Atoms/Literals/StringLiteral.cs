namespace Zuh.Compiler.Ast {
    public record StringLiteral : Literal {
        public required string Value { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            yield break;
        }
    }
}