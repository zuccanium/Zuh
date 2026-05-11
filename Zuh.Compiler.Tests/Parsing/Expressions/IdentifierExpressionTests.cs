using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing.Expressions {
    public class IdentifierExpressionTests {
        [Fact]
        public void Parse_ValidIdentifierExpression_Works() {
            Resolve(Mark(out var identifierExpressionMarker, $"{CreateIdentifier(out var identifierGetter)}"));

            var result = ZuhParser.Expression.Parse(identifierExpressionMarker.Value);

            var expected = new IdentifierExpression() {
                Identifier = identifierGetter(),
                SourceSpan = identifierExpressionMarker.SourceSpan
            };
            
            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }
    }
}