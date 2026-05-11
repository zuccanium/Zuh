using Zuh.Compiler.Ast;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;

namespace Zuh.Compiler.Tests.Infrastructure {
    public static partial class SyntaxFactory {
        public static StringLiteral StringLiteralPlaceholder
            => new() { Value = Placeholder };
        
        public static MappingNode CreateStringLiteral(out Func<StringLiteral> getter, StringLiteral value) {
            var node = Mark(out var stringLiteral, $"\"{value.Value}\"");

            getter = () => value with {
                SourceSpan = stringLiteral.SourceSpan
            };

            return node;
        }

        public static MappingNode CreateStringLiteral(out Func<StringLiteral> getter, string value)
            => CreateStringLiteral(out getter, new StringLiteral() {
                Value = value
            });

        public static MappingNode CreateStringLiteral(out Func<StringLiteral> getter)
            => CreateStringLiteral(out getter, StringLiteralPlaceholder);
    }
}