using Zuh.Compiler.Diagnostics;
using Zuh.Compiler.Semantics.Types;

namespace Zuh.Compiler.Semantics.Diagnostics {
    public record InvalidFunctionArgumentError : Error {
        public required string ParameterName { get; init; }
        public required ZuhType ExpectedType { get; init; }
        public required ZuhType ProvidedType { get; init; }

        public override string Message
            => $"expected expression of type {ExpectedType.String} instead of {ProvidedType} for parameter {ParameterName}";
    }
}