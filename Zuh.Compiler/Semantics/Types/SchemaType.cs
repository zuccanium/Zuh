namespace Zuh.Compiler.Semantics.Types {
    public record SchemaType : ZuhType {
        public override string String
            => "schema";
    }
}