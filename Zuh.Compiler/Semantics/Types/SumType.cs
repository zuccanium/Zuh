namespace Zuh.Compiler.Semantics.Types {
    public record SumType : ZuhType {
        public override string String
            => "sum";
    }
}