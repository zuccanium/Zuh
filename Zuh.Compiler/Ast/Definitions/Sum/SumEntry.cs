namespace Zuh.Compiler.Ast {
    public record SumEntry : ZuhNode {
        public required Label Name { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            yield return Name;
        }
    }
}