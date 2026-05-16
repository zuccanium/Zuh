using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using Zuh.Compiler.Tests.Infrastructure.Extensions;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing.Expressions {
    public class ExpressionTests {
        private void createNExpressions(int n, out MappingNode[] nodes, out Func<Expression>[] getters) {
            nodes = Enumerable.Range(0, n)
                .SelectWithOut(
                    out var gettersEnumerable,
                    (int _, out Func<Expression> outValue)
                        => CreateExpression(out outValue)
                )
                .ToArray();

            getters = gettersEnumerable
                .ToArray();
        }

        [Fact]
        public void Parse_IntersectionUnion_IntersectionIsHigherThanUnion() {
            createNExpressions(3, out var nodes, out var getters);
            
            Resolve(Mark(out var expressionMarker, $"{nodes[0]} & {nodes[1]} | {nodes[2]}"));

            var result = ZuhParser.Expression.Parse(expressionMarker.Value);

            var expected = new UnionExpression() {
                Left = new IntersectionExpression() {
                    Left = getters[0](),
                    Right = getters[1](),
                    SourceSpan = getters[0]().SourceSpan - getters[1]().SourceSpan
                },
                Right = getters[2](),
                SourceSpan = expressionMarker.SourceSpan
            };
            
            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }
        
        [Fact]
        public void Parse_UnionIntersection_IntersectionIsHigherThanUnion() {
            createNExpressions(3, out var nodes, out var getters);
            
            Resolve(Mark(out var expressionMarker, $"{nodes[0]} | {nodes[1]} & {nodes[2]}"));

            var result = ZuhParser.Expression.Parse(expressionMarker.Value);

            var expected = new UnionExpression() {
                Left = getters[0](),
                Right = new IntersectionExpression() {
                    Left = getters[1](),
                    Right = getters[2](),
                    SourceSpan = getters[1]().SourceSpan - getters[2]().SourceSpan
                },
                SourceSpan = expressionMarker.SourceSpan
            };
            
            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }
        
        [Fact]
        public void Parse_IntersectionArray_ArrayIsHigherThanIntersection() {
            createNExpressions(2, out var nodes, out var getters);
            
            Resolve(Mark(out var expressionMarker, $"{nodes[0]} & {Mark(out var arrayExpressionMarker, $"{nodes[1]}[]")}"));

            var result = ZuhParser.Expression.Parse(expressionMarker.Value);

            var expected = new IntersectionExpression() {
                Left = getters[0](),
                Right = new ArrayExpression() {
                    Expression = getters[1](),
                    SourceSpan = arrayExpressionMarker.SourceSpan
                },
                SourceSpan = expressionMarker.SourceSpan
            };
            
            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }
    }
}