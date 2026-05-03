namespace Zuh.Compiler.Ast {
    public record SumEntry : ZuhNode {
        public required Key Key { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            yield return Key;
        }
    }
}