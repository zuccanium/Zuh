using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using Zuh.Compiler.Tests.Infrastructure.Extensions;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing.Expressions {
    public class FunctionInvocationExpressionTests {
        public class Parse_ValidFunctionInvocationExpression_Works_Data : TheoryData<string, FunctionInvocationExpression> {
            public Parse_ValidFunctionInvocationExpression_Works_Data() {
                for(var i = 0; i < 4; i++)
                    add(i);
            }

            private void add(int argumentCount) {
                var argumentNodes = Enumerable.Range(0, argumentCount)
                    .SelectWithOut(
                        out var argumentGetters,
                        (int n, out Func<Expression> outValue)
                            => CreateExpression(out outValue)
                    );

                var identifierNode = CreateIdentifier(out var identifierGetter);
                Resolve(Mark(out var functionInvocationExpressionMarker, $"{identifierNode}({argumentNodes.MarkAsJoined(", ")})"));
                
                Add(
                    functionInvocationExpressionMarker.Value,
                    new FunctionInvocationExpression() {
                        FunctionIdentifier = identifierGetter(),
                        Arguments = [
                            ..argumentGetters
                                .Select(getter => getter())
                        ],
                        SourceSpan = functionInvocationExpressionMarker.SourceSpan,
                    }
                );
            }
        }
        
        [Theory]
        [ClassData(typeof(Parse_ValidFunctionInvocationExpression_Works_Data))]
        public void Parse_ValidFunctionInvocationExpression_Works(string value, FunctionInvocationExpression expected) {
            var result = ZuhParser.Expression.Parse(value);
            
            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }
        
        [Theory]
        [InlineData("")]
        [InlineData("()")]
        public void Parse_InvalidFunctionInvocationExpression_Fails(string value) {
            var result = ZuhParser.Expression.Parse(value);
            
            Assert.False(result.Success);
        }
        
        [Theory]
        [InlineData("noArguments")]
        public void Parse_NotFunctionInvocationExpression_IsOtherThing(string value) {
            var result = ZuhParser.Expression.Parse(value);
            
            Assert.IsNotType<FunctionInvocationExpression>(result.Value);
        }
    }
}