using Zuh.Compiler.Diagnostics;

namespace Zuh.Compiler.Semantics.Diagnostics {
    /// <summary>
    /// represents an error that occurs when a module fails to resolve.
    /// </summary>
    public record ModuleResolutionError : Error {
        public required string ModuleName { get; init; }

        public override string Message
            => $"failed to resolve module {ModuleName}";
    }
}