using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using Zuh.Compiler.Tests.Infrastructure.Extensions;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing.Statements.Declarations {
    public class ExpressionDeclarationTests {
        public class Parse_ValidExpressionDeclaration_Works_Data : TheoryData<string, ExpressionDeclaration> {
            public Parse_ValidExpressionDeclaration_Works_Data() {
                for(var i = 0; i < 5; i++) {
                    add(i, false);
                    add(i, true);
                }
            }
            
            private void add(int documentationCount, bool isExport) {
                var expressionNode = CreateExpression(out var expressionGetter);

                DeclarationTests.AddDefinitionDeclaration(
                    documentationCount,
                    isExport,
                    expressionNode,
                    Add,
                    (name) => new ExpressionDeclaration() {
                        Name = name,
                        Expression = expressionGetter()
                    }
                );
            }
        }
        
        [Theory]
        [ClassData(typeof(Parse_ValidExpressionDeclaration_Works_Data))]
        public void Parse_ValidExpressionDeclarationWithoutExport_Works(string value, ExpressionDeclaration expected) {
            var result = ZuhParser.ExpressionDeclaration.Parse(value);
            
            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }
    }
}