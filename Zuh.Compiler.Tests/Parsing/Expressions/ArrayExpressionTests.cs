using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing.Expressions {
    public class ArrayExpressionTests {
        [Fact]
        public void Parse_ValidArrayExpression_Works() {
            Resolve(Mark(out var arrayExpressionMarker, $"{CreateExpression(out var expressionGetter)}[]"));

            var result = ZuhParser.Expression.Parse(arrayExpressionMarker.Value);

            var expected = new ArrayExpression() {
                Expression = expressionGetter(),
                SourceSpan = arrayExpressionMarker.SourceSpan
            };
            
            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }

        [Theory]
        [InlineData("[]")]
        public void Parse_NotArrayExpression_IsOtherThing(string value) {
            var result = ZuhParser.Expression.Parse(value);
            
            Assert.IsNotType<ArrayExpression>(result.Value);
        }
    }
}