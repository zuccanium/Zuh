using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing {
    public class ImportStatementTests {
        [Fact]
        public void Parse_ValidImportStatement_Works() {
            Resolve(Mark(out var importStatement, $"import {CreateStringLiteral(out var stringLiteralGetter)};"));

            var result = ZuhParser.ImportStatement.Parse(importStatement.Value);
            
            var expected = new ImportStatement() {
                Module = stringLiteralGetter(),
                DocumentationLines = [],
                SourceSpan = importStatement.SourceSpan
            };
            
            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }
        
        [Theory]
        [InlineData("import;")]
        [InlineData("import identifier;")]
        [InlineData("import 1;")]
        [InlineData("import \"no_semicolon\"")]
        public void Parse_InvalidImportStatement_Fails(string value) {
            var result = ZuhParser.ImportStatement.Parse(value);
            
            Assert.False(result.Success);
        }
    }
}