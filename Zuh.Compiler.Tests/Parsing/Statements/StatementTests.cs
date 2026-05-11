using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing {
    public class StatementTests {
        [Fact]
        public void Parse_ValidDeclarationStatement_CreatesDeclaration() {
            Resolve(Mark(out var declarationMarker, $"{CreateDeclaration(out _)}"));
            
            var result = ZuhParser.Statement.Parse(declarationMarker.Value);
            
            Assert.True(result.Success);
            Assert.IsType<Declaration>(result.Value, exactMatch: false);
        }
        
        [Fact]
        public void Parse_ValidImportStatementStatement_CreatesImportStatement() {
            Resolve(Mark(out var importStatementMarker, $"{CreateImportStatement(out _)}"));
            
            var result = ZuhParser.Statement.Parse(importStatementMarker.Value);
            
            Assert.True(result.Success);
            Assert.IsType<ImportStatement>(result.Value, exactMatch: false);
        }
    }
}