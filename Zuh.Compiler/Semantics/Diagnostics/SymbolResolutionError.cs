using Zuh.Compiler.Diagnostics;

namespace Zuh.Compiler.Semantics.Diagnostics {
    /// <summary>
    /// error that occurs when a symbol fails to resolve.
    /// </summary>
    public record SymbolResolutionError : Error {
        public required string SymbolName { get; init; }

        public override string Message
            => $"failed to resolve symbol {SymbolName}";
    }
}