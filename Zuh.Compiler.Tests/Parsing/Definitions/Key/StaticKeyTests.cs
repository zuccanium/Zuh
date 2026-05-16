using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using Zuh.Compiler.Tests.Infrastructure.Extensions;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing.Definitions.Key {
    public class StaticKeyTests {
        public class Parse_ValidStaticKey_Works_Data : TheoryData<string, StaticKey> {
            public Parse_ValidStaticKey_Works_Data() {
                {
                    Resolve(Mark(out var functionParameter, $"{CreateLabel(out var labelGetter)}"));
                    
                    Add(
                        functionParameter.Value,
                        new StaticKey() {
                            Name = labelGetter(),
                            SourceSpan = functionParameter.SourceSpan
                        }
                    );
                }

                {
                    Resolve(Mark(out var functionParameter, $"{CreateLabel(out var labelGetter)}?"));
                    
                    Add(
                        functionParameter.Value,
                        new StaticKey() {
                            IsOptional = true,
                            Name = labelGetter(),
                            SourceSpan = functionParameter.SourceSpan
                        }
                    );
                }
            }
        }

        [Theory]
        [ClassData(typeof(Parse_ValidStaticKey_Works_Data))]
        public void Parse_ValidStaticKey_Works(string value, StaticKey expected) {
            var result = ZuhParser.StaticKey.Parse(value);
            
            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }

        [Theory]
        [InlineData("?")]
        [InlineData("")]
        public void Parse_InvalidStaticKey_Fails(string value) {
            var result = ZuhParser.StaticKey.Parse(value);
            
            Assert.False(result.Success);
        }
    }
}