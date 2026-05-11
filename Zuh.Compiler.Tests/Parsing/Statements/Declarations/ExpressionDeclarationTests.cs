using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing.Statements.Declarations {
    public class ExpressionDeclarationTests {
        [Fact]
        public void Parse_ValidExpressionDeclarationWithoutExport_Works() {
            var labelNode = CreateLabel(out var labelGetter);
            var expressionNode = CreateExpression(out var expressionGetter);
            
            Resolve(Mark(out var expressionDeclarationMarker, $"{labelNode} {expressionNode};"));
            
            var result = ZuhParser.ExpressionDeclaration.Parse(expressionDeclarationMarker.Value);

            var expected = new ExpressionDeclaration() {
                Name = labelGetter(),
                Expression = expressionGetter(),
                DocumentationLines = [],
                SourceSpan = expressionDeclarationMarker.SourceSpan
            };
            
            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }
        
        [Fact]
        public void Parse_ValidExpressionDeclarationWithExport_Works() {
            var labelNode = CreateLabel(out var labelGetter);
            var expressionNode = CreateExpression(out var expressionGetter);
            
            Resolve(Mark(out var expressionDeclarationMarker, $"export {labelNode} {expressionNode};"));
            
            var result = ZuhParser.ExpressionDeclaration.Parse(expressionDeclarationMarker.Value);

            var expected = new ExpressionDeclaration() {
                IsExport = true,
                Name = labelGetter(),
                Expression = expressionGetter(),
                DocumentationLines = [],
                SourceSpan = expressionDeclarationMarker.SourceSpan
            };
            
            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }
    }
}