using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using Zuh.Compiler.Tests.Infrastructure.Extensions;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing.Statements.Declarations {
    public class DeclarationTests {
        [Fact]
        public void Parse_ValidExpressionDeclarationDeclaration_CreatesExpressionDeclaration() {
            Resolve(Mark(out var expressionDeclarationMarker, $"{CreateExpressionDeclaration(out _)}"));
            
            var result = ZuhParser.Declaration.Parse(expressionDeclarationMarker.Value);
            
            Assert.True(result.Success);
            Assert.IsType<ExpressionDeclaration>(result.Value, exactMatch: false);
        }
        
        [Fact]
        public void Parse_ValidFunctionDeclarationDeclaration_CreatesFunctionDeclaration() {
            Resolve(Mark(out var functionDeclarationMarker, $"{CreateFunctionDeclaration(out _)}"));
            
            var result = ZuhParser.Declaration.Parse(functionDeclarationMarker.Value);
            
            Assert.True(result.Success);
            Assert.IsType<FunctionDeclaration>(result.Value, exactMatch: false);
        }
    }
}