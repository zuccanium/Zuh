using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing.Definitions.Key {
    public class KeyTests {
        [Fact]
        public void Parse_ValidDynamicKeyKey_CreatesDynamicKey() {
            Resolve(Mark(out var keyMarker, $"{CreateExpressionKey(out _)}"));
            
            var result = ZuhParser.Key.Parse(keyMarker.Value);
            
            Assert.True(result.Success);
            Assert.IsType<DynamicKey>(result.Value, exactMatch: false);
        }
        
        [Fact]
        public void Parse_ValidStaticKeyKey_CreatesStaticKey() {
            Resolve(Mark(out var keyMarker, $"{CreateStaticKey(out _)}"));
            
            var result = ZuhParser.Key.Parse(keyMarker.Value);
            
            Assert.True(result.Success);
            Assert.IsType<StaticKey>(result.Value, exactMatch: false);
        }
    }
}