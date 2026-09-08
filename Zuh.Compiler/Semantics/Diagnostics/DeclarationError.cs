using Zuh.Compiler.Diagnostics;

namespace Zuh.Compiler.Semantics.Diagnostics {
     /// <summary>
    /// represents an error that occurs when trying to declare something with the same name as something else.
    /// </summary>
    public record DeclarationError : Error {
        public required string DeclarationName { get; init; }

        public override string Message
            => $"{DeclarationName} is already declared";
    }
}