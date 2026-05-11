using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing.Definitions.Key {
    public class DynamicKeyTests {
        [Fact]
        public void Parse_ValidExpressionDynamicKey_CreatesExpression() {
            Resolve(Mark(out var dynamicKeyMarker, $"{CreateExpressionKey(out _)}"));
            
            var result = ZuhParser.DynamicKey.Parse(dynamicKeyMarker.Value);
            
            Assert.True(result.Success);
            Assert.IsType<ExpressionKey>(result.Value);
        }
    }
}