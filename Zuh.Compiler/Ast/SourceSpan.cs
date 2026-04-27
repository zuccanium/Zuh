namespace Zuh.Compiler.Ast {
    public record struct SourceSpan {
        public required int Start { get; init; }
        public required int End { get; init; }
    }
}