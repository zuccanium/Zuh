using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing.Expressions {
    public class SchemaExpressionTests {
        [Fact]
        public void Parse_ValidSchemaExpression_Works() {
            Resolve(Mark(out var schemaExpressionMarker, $"{CreateSchema(out var schemaGetter)}"));

            var result = ZuhParser.Expression.Parse(schemaExpressionMarker.Value);

            var expected = new SchemaExpression() {
                Schema = schemaGetter(),
                SourceSpan = schemaExpressionMarker.SourceSpan
            };
            
            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }
    }
}