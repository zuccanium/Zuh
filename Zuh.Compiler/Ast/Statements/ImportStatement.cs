namespace Zuh.Compiler.Ast {
    public record ImportStatement : Statement {
        public required StringLiteral Module { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            yield return Module;
        }
    }
}