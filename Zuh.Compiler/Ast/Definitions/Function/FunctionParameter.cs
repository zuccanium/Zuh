namespace Zuh.Compiler.Ast {
    public record FunctionParameter : ZuhNode {
        public required Identifier Name { get; init; }
        public required FunctionParameterType Type { get; init; }

        public enum FunctionParameterType {
            Keys,
            Schema
        }
    }
}