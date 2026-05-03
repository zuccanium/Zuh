using Zuh.Compiler.Diagnostics;

namespace Zuh.Compiler.Semantics.Diagnostics {
    public record ResolutionError : Error {
        public required string Name { get; init; }
    }
}