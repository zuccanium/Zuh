using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;

namespace Zuh.Compiler.Tests.Parsing.Expressions {
    public class IntersectionExpressionTests {
        private const string OperatorString = "&";

        public class Parse_InvalidIntersectionExpression_Fails_Data : TheoryData<string> {
            public Parse_InvalidIntersectionExpression_Fails_Data()
                => BinaryExpressionTests.AddFailingCases(OperatorString, str => Add(str));
        }
        
        [Fact]
        public void Parse_ValidIntersectionExpression_Works() {
            BinaryExpressionTests.Parse(
                OperatorString,
                out var leftGetter,
                out var rightGetter,
                out var binaryExpressionMarker,
                out var result
            );

            var expected = new IntersectionExpression() {
                Left = leftGetter(),
                Right = rightGetter(),
                SourceSpan = binaryExpressionMarker.SourceSpan
            };
            
            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }
        
        [Theory]
        [ClassData(typeof(Parse_InvalidIntersectionExpression_Fails_Data))]
        public void Parse_InvalidIntersectionExpression_Fails(string value) {
            var result = ZuhParser.Expression.Parse(value);
            
            Assert.False(result.Success);
        }
    }
}