namespace Zuh.Compiler.Semantics.Types {
    public record FunctionType : ZuhType {
        public required ZuhType ReturnType { get; init; }
        public required List<ZuhType> ParameterTypes { get; init; }
        
        public override string String
            => $"({from parameterType in ParameterTypes select String}) {ReturnType.String})";
    }
}