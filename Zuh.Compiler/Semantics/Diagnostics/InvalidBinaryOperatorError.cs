using Zuh.Compiler.Diagnostics;
using Zuh.Compiler.Semantics.Types;

namespace Zuh.Compiler.Semantics.Diagnostics {
    public record InvalidBinaryOperatorError : Error {
        public required ZuhType LeftType { get; init; }
        public required ZuhType RightType { get; init; }
        public required string Operator { get; init; }

        public override string Message
            => $"operator {Operator} cannot be applied between {LeftType.String} and {RightType.String}";
    }
}