using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;

namespace Zuh.Compiler.Tests.Parsing.Atoms {
    public class IdentifierTests {
        [Theory]
        [InlineData("PascalCase")]
        [InlineData("camelCase")]
        [InlineData("snake_case")]
        [InlineData("_freaky_case")]
        [InlineData("WeAreNumber1")]
        [InlineData("MyPasswordIs0123456789")]
        public void Parse_ValidIdentifier_Works(string value) {
            var result = ZuhParser.Identifier.Parse(value);
            
            Assert.True(result.Success);

            var expected = new Identifier() {
                Value = value,
                SourceSpan = new SourceSpan() {
                    Start = 0,
                    End = value.Length
                }
            };
            
            var actual = result.Value;
            
            Assert.Equivalent(expected, actual);
        }
        
        [Theory]
        [InlineData("something", "")]
        [InlineData("something", " ")]
        [InlineData("something", "   ")]
        public void Parse_ValidIdentifier_HasCorrectSourceSpan(string value, string padding) {
            var result = ZuhParser.Identifier.Parse(value + padding);
            
            Assert.True(result.Success);

            var expected = new Identifier() {
                Value = value,
                SourceSpan = new SourceSpan() {
                    Start = 0,
                    End = value.Length
                }
            };
            
            var actual = result.Value;
            
            Assert.Equivalent(expected, actual);
        }
        
        [Theory]
        [InlineData("1beeOutsideYourHouse")]
        [InlineData("2beesOutsideYourHouse")]
        [InlineData("3beesOutsideYourHouse")]
        [InlineData("4beesItsProbablyFine")]
        public void Parse_InvalidIdentifier_DoesntWork(string value) {
            var result = ZuhParser.Identifier.Parse(value);
            
            Assert.False(result.Success);
        }
    }
}