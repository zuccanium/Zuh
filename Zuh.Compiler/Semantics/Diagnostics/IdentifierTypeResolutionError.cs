using Zuh.Compiler.Diagnostics;

namespace Zuh.Compiler.Semantics.Diagnostics {
    public record IdentifierTypeResolutionError : Error {
        public required string Identifier { get; init; }

        public override string Message
            => $"could not resolve type of identifier {Identifier}";
    }
}