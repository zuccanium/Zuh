using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing.Statements.Declarations {
    public class FunctionDeclarationTests {
        public class Parse_ValidFunctionDeclaration_Works_Data : TheoryData<string, FunctionDeclaration> {
            public Parse_ValidFunctionDeclaration_Works_Data() {
                for(var i = 0; i < 5; i++) {
                    add(i, false);
                    add(i, true);
                }
            }
            
            private void add(int documentationCount, bool isExport) {
                var functionNode = CreateFunction(out var functionGetter);

                DeclarationTests.AddDefinitionDeclaration(
                    documentationCount,
                    isExport,
                    functionNode,
                    Add,
                    (name) => new FunctionDeclaration() {
                        Name = name,
                        Function = functionGetter()
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