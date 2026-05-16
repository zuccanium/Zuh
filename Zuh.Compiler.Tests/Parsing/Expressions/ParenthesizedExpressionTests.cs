using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing.Expressions {
    public class ParenthesizedExpressionTests {
        [Fact]
        public void Parse_ValidParenthesizedExpression_Works() {
            Resolve(Mark(out var parenthesizedExpressionMarker, $"({CreateExpression(out var expressionGetter)})"));

            var result = ZuhParser.Expression.Parse(parenthesizedExpressionMarker.Value);
            
            var expected = new ParenthesizedExpression() {
                Expression = expressionGetter(),
                SourceSpan = parenthesizedExpressionMarker.SourceSpan
            };
            
            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }
        
        [Theory]
        [InlineData("()")]
        [InlineData(")")]
        [InlineData("(")]
        public void Parse_InvalidParenthesizedExpression_Fails(string value) {
            var result = ZuhParser.Expression.Parse(value);

            Assert.False(result.Success);
        }
    }
}