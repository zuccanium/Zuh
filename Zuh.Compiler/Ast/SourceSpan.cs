namespace Zuh.Compiler.Ast {
    public record struct SourceSpan {
        public required int Start { get; init; }
        public required int End { get; init; }

        /// <summary>
        /// creates a new <see cref="SourceSpan"/> that spans the length between and including both.
        /// </summary>
        public static SourceSpan operator -(SourceSpan a, SourceSpan b)
            => new() {
                Start = Math.Min(a.Start, b.Start),
                End = Math.Max(a.End, b.End)
            };
    }
}