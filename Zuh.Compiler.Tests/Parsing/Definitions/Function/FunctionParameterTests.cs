using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;

namespace Zuh.Compiler.Tests.Parsing.Definitions.Function {
    public class FunctionParameterTests {
        [Theory]
        [InlineData("name schema", "name", FunctionParameter.FunctionParameterType.Schema)]
        [InlineData("name sum", "name", FunctionParameter.FunctionParameterType.Sum)]
        public void Parse_ValidFunctionParameter_Works(string value, string name, FunctionParameter.FunctionParameterType type) {
            var result = ZuhParser.FunctionParameter.Parse(value);
            
            Assert.True(result.Success);

            var expected = new FunctionParameter() {
                Name = new Label() {
                    Value = name,
                    SourceSpan = new SourceSpan() {
                        Start = 0,
                        End = name.Length
                    }
                },
                Type = type,
                SourceSpan = new SourceSpan() {
                    Start = 0,
                    End = value.Length
                }
            };

            var actual = result.Value;
            
            Assert.Equivalent(expected, actual);
        }
    }
}