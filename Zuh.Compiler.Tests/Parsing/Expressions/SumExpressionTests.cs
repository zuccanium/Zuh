using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing.Expressions {
    public class SumExpressionTests {
        [Fact]
        public void Parse_ValidSumExpression_Works() {
            Resolve(Mark(out var sumExpressionMarker, $"{CreateSum(out var sumGetter)}"));

            var result = ZuhParser.Expression.Parse(sumExpressionMarker.Value);

            var expected = new SumExpression() {
                Sum = sumGetter(),
                SourceSpan = sumExpressionMarker.SourceSpan
            };
            
            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }
    }
}