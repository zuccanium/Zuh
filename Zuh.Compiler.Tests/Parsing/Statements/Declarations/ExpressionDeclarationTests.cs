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

                var documentationLineNodes = Enumerable.Range(0, documentationCount)
                    .SelectWithOut(
                        out var documentationLineGetters,
                        (int _, out Func<DocumentationLine> outValue)
                            => CreateDocumentationLine(out outValue)
                    );
                
                var labelNode = CreateLabel(out var labelGetter);

                var node = isExport
                    ? Mark(
                        out var expressionDeclarationMarker,
                        $"{documentationLineNodes.MarkAsJoined("\n", true)}export {labelNode} {expressionNode};")
                    
                    : Mark(
                        out expressionDeclarationMarker,
                        $"{documentationLineNodes.MarkAsJoined("\n", true)}{labelNode} {expressionNode};");
            
                Resolve(node);

                Add(
                    expressionDeclarationMarker.Value,
                    new ExpressionDeclaration() {
                        Name = labelGetter(),
                        Expression = expressionGetter(),
                        IsExport = isExport,
                        DocumentationLines = [
                            ..documentationLineGetters
                                .Select(getter => getter())
                        ],
                        SourceSpan = expressionDeclarationMarker.SourceSpan
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