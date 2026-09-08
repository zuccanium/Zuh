namespace Zuh.Compiler.Semantics.Types {
    public record ArrayType : ZuhType {
        public required ZuhType Inner { get; init; }
        
        public override string String
            => $"{Inner.String}[]";
    }
}