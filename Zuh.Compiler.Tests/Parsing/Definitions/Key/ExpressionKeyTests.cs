using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing.Definitions.Key {
    public class ExpressionKeyTests {
        [Fact]
        public void Parse_ValidExpressionKey_Works() {
            Resolve(Mark(out var expressionKeyMarker, $"<{CreateExpression(out var expressionGetter)}>"));
            
            var result = ZuhParser.ExpressionKey.Parse(expressionKeyMarker.Value);
            
            var expected = new ExpressionKey() {
                Expression = expressionGetter(),
                SourceSpan = expressionKeyMarker.SourceSpan
            };
            
            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }
    }
}