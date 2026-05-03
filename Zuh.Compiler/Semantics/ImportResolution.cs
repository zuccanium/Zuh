namespace Zuh.Compiler.Semantics {
    public record ImportResolution {
        public required bool Success { get; init; }
        public string? Id { get; init; }
    }
}