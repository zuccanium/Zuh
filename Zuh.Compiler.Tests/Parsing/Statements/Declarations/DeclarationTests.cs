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

        public static void AddDefinitionDeclaration<TDeclaration>(
            int documentationCount,
            bool isExport,
            MappingNode definitionNode,
            Action<string, TDeclaration> add,
            Func<Label, TDeclaration> declarationCreator
        ) where TDeclaration : Declaration {
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
                    $"{documentationLineNodes.MarkAsJoined("\n", true)}export {labelNode} {definitionNode};")
                    
                : Mark(
                    out expressionDeclarationMarker,
                    $"{documentationLineNodes.MarkAsJoined("\n", true)}{labelNode} {definitionNode};");
            
            Resolve(node);

            add(
                expressionDeclarationMarker.Value,
                declarationCreator(labelGetter()) with {
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
}