using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using Zuh.Compiler.Tests.Infrastructure.Extensions;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing.Definitions.Schema {
    public class SchemaEntryTests {
        [Fact]
        public void Parse_ValidSchemaEntryWithoutValue_Works() {
            Resolve(Mark(out var schemaEntryMarker, $"{CreateKey(out var keyGetter)}"));
            
            var result = ZuhParser.SchemaEntry.Parse(schemaEntryMarker.Value);

            var expected = new SchemaEntry() {
                Key = keyGetter(),
                DocumentationLines = [],
                SourceSpan = schemaEntryMarker.SourceSpan
            };
            
            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }
        
        [Fact]
        public void Parse_ValidSchemaEntryWithValue_Works() {
            Resolve(Mark(out var schemaEntryMarker, $"{CreateKey(out var keyGetter)} {CreateExpression(out var valueGetter)}"));
            
            var result = ZuhParser.SchemaEntry.Parse(schemaEntryMarker.Value);

            var expected = new SchemaEntry() {
                Key = keyGetter(),
                Value = valueGetter(),
                DocumentationLines = [],
                SourceSpan = schemaEntryMarker.SourceSpan
            };
            
            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }
    }
}