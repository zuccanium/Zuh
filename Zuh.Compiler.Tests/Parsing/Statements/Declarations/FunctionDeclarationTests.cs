using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing.Statements.Declarations {
    public class FunctionDeclarationTests {
        public class Parse_ValidFunctionDeclaration_Works_Data : TheoryData<string, FunctionDeclaration> {
            public Parse_ValidFunctionDeclaration_Works_Data() {
                add(false);
                add(true);
            }
            
            private void add(bool isExport) {
                var functionNode = CreateFunction(out var functionGetter);

                var labelNode = CreateLabel(out var labelGetter);

                var node = isExport
                    ? Mark(
                        out var functionDeclarationMarker,
                        $"export {labelNode} {functionNode};")
                    
                    : Mark(
                        out functionDeclarationMarker,
                        $"{labelNode} {functionNode};");
            
                Resolve(node);

                Add(
                    functionDeclarationMarker.Value,
                    new FunctionDeclaration() {
                        Name = labelGetter(),
                        Function = functionGetter(),
                        IsExport = isExport,
                        SourceSpan = functionDeclarationMarker.SourceSpan
                    }
                );
            }
        }
        
        [Theory]
        [ClassData(typeof(Parse_ValidFunctionDeclaration_Works_Data))]
        public void Parse_ValidExpressionDeclarationWithoutExport_Works(string value, FunctionDeclaration expected) {
            var result = ZuhParser.FunctionDeclaration.Parse(value);
            
            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }
    }
}