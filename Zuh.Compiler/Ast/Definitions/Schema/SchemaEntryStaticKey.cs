namespace Zuh.Compiler.Ast {
    public record SchemaEntryStaticKey : SchemaEntryKey {
        public required Label Name { get; init; }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            yield return Name;
        }
    }
}