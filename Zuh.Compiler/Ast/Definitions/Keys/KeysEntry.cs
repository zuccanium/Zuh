namespace Zuh.Compiler.Ast {
    public record KeysEntry : ZuhNode {
        public required Label Name { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            yield return Name;
        }
    }
}