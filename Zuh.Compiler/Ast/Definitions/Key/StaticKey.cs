namespace Zuh.Compiler.Ast {
    public record StaticKey : Key {
        public required Label Name { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            yield return Name;
        }
    }
}