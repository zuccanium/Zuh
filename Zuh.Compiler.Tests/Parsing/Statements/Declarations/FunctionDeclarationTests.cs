using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing.Statements.Declarations {
    public class FunctionDeclarationTests {
        [Fact]
        public void Parse_ValidFunctionDeclarationWithoutExport_Works() {
            var labelNode = CreateLabel(out var labelGetter);
            var functionNode = CreateFunction(out var functionGetter);
            
            Resolve(Mark(out var functionDeclarationMarker, $"{labelNode} {functionNode};"));
            
            var result = ZuhParser.FunctionDeclaration.Parse(functionDeclarationMarker.Value);

            var expected = new FunctionDeclaration() {
                Name = labelGetter(),
                Function = functionGetter(),
                DocumentationLines = [],
                SourceSpan = functionDeclarationMarker.SourceSpan
            };
            
            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }
        
        [Fact]
        public void Parse_ValidFunctionDeclarationWithExport_Works() {
            var labelNode = CreateLabel(out var labelGetter);
            var functionNode = CreateFunction(out var functionGetter);
            
            Resolve(Mark(out var functionDeclarationMarker, $"export {labelNode} {functionNode};"));
            
            var result = ZuhParser.FunctionDeclaration.Parse(functionDeclarationMarker.Value);

            var expected = new FunctionDeclaration() {
                IsExport = true,
                Name = labelGetter(),
                Function = functionGetter(),
                DocumentationLines = [],
                SourceSpan = functionDeclarationMarker.SourceSpan
            };
            
            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }
    }
}