using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using Zuh.Compiler.Tests.Infrastructure.Extensions;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing.Definitions.Function {
    public class FunctionTests {
        public class Parse_ValidFunction_Works_Data : TheoryData<string, Ast.Function> {
            public Parse_ValidFunction_Works_Data() {
                // surely i dont need to go any higher than this
                for(var i = 0; i < 4; i++)
                    add(i);
            }

            private void add(int parameterCount) {
                var parameterNodes = Enumerable.Range(0, parameterCount)
                    .SelectWithOut(
                        out var parameterGetters,
                        (int n, out Func<FunctionParameter> outValue)
                            => CreateFunctionParameter(out outValue)
                    );
                
                Resolve(Mark(out var function, $"({parameterNodes.MarkAsJoined(", ")}) {CreateExpression(out var expressionGetter)}"));
                
                Add(
                    function.Value,
                    new Ast.Function() {
                        Parameters = [
                            ..parameterGetters
                                .Select(getter => getter())
                        ],
                        Expression = expressionGetter(),
                        SourceSpan = function.SourceSpan
                    }
                );
            }
        }
        
        [Theory]
        [ClassData(typeof(Parse_ValidFunction_Works_Data))]
        public void Parse_ValidFunction_Works(string value, Ast.Function expected) {
            var result = ZuhParser.Function.Parse(value);
            
            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }
        
        [Theory]
        [InlineData("( {}")]
        [InlineData(") {}")]
        [InlineData("{}")]
        [InlineData("buh {}")]
        [InlineData("[] {}")]
        [InlineData("<> {}")]
        public void Parse_FunctionWithInvalidParameters_Fails(string value) {
            var result = ZuhParser.Function.Parse(value);
            
            Assert.False(result.Success);
        }
        
        [Theory]
        [InlineData("()")]
        [InlineData("() ()")]
        [InlineData("() <>")]
        public void Parse_FunctionWithInvalidExpression_Fails(string value) {
            var result = ZuhParser.Function.Parse(value);
            
            Assert.False(result.Success);
        }
    }
}