namespace Zuh.Compiler.Ast {
    public record FunctionParameter : ZuhNode {
        public required Label Name { get; init; }
        public required FunctionParameterType Type { get; init; }

        public enum FunctionParameterType {
            Keys,
            Schema
        }

        public override IEnumerator<IZuhNode> GetChildrenEnumerator() {
            yield return Name;
        }
    }
}