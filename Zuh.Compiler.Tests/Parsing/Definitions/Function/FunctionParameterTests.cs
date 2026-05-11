using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing.Definitions.Function {
    public class FunctionParameterTests {
        public class Parse_ValidFunctionParameter_Works_Data : TheoryData<string, FunctionParameter> {
            public Parse_ValidFunctionParameter_Works_Data() {
                add("sum", FunctionParameter.FunctionParameterType.Sum);
                add("schema", FunctionParameter.FunctionParameterType.Schema);
            }

            private void add(string name, FunctionParameter.FunctionParameterType type) {
                Resolve(Mark(out var functionParameter, $"{CreateLabel(out var labelGetter)} {name}"));
                    
                Add(
                    functionParameter.Value,
                    new FunctionParameter() {
                        Name = labelGetter(),
                        Type = type,
                        SourceSpan = functionParameter.SourceSpan
                    }
                );
            }
        }
        
        [Theory]
        [ClassData(typeof(Parse_ValidFunctionParameter_Works_Data))]
        public void Parse_ValidFunctionParameter_Works(string value, FunctionParameter expected) {
            var result = ZuhParser.FunctionParameter.Parse(value);
            
            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }
        
        [Theory]
        [InlineData("withoutType")]
        [InlineData("invalidType buh")]
        public void Parse_InvalidFunctionParameter_Fails(string value) {
            var result = ZuhParser.FunctionParameter.Parse(value);
            
            Assert.False(result.Success);
        }
    }
}