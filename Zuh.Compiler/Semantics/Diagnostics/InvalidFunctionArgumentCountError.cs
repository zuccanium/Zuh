using Zuh.Compiler.Diagnostics;

namespace Zuh.Compiler.Semantics.Diagnostics {
    public record InvalidFunctionArgumentCountError : Error {
        public required int ParameterCount { get; init; }
        public required int ArgumentCount { get; init; }
        public required string FunctionName { get; init; }

        public override string Message
            => $"expected {ParameterCount} arguments but got {ArgumentCount} while invoking function {FunctionName}";
    }
}