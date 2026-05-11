using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using Zuh.Compiler.Tests.Infrastructure.Extensions;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing.Definitions.Schema {
    public class SchemaEntryTests {
        public class Parse_ValidSchemaEntry_Works_Data : TheoryData<string, SchemaEntry> {
            public Parse_ValidSchemaEntry_Works_Data() {
                for(var i = 0; i < 5; i++) {
                    add(i, true);
                    add(i, false);
                }
            }

            private void add(int documentationCount, bool hasValue) {
                var documentationLineNodes = Enumerable.Range(0, documentationCount)
                    .SelectWithOut(
                        out var documentationLineGetters,
                        (int _, out Func<DocumentationLine> outValue)
                            => CreateDocumentationLine(out outValue)
                    );

                var valueGetter = default(Func<Expression>);

                var keyNode = CreateKey(out var keyGetter);
                
                var node = hasValue
                    ? Mark(
                        out var schemaEntryMarker,
                        $"{documentationLineNodes.MarkAsJoined("\n", true)}{keyNode} {CreateExpression(out valueGetter)}")
                    
                    : Mark(
                        out schemaEntryMarker,
                        $"{documentationLineNodes.MarkAsJoined("\n", true)}{keyNode}");
                
                Resolve(node);
                
                Add(
                    schemaEntryMarker.Value,
                    new SchemaEntry() {
                        Key = keyGetter(),
                        Value = valueGetter?.Invoke(),
                        DocumentationLines = [
                            ..documentationLineGetters
                                .Select(getter => getter())
                        ],
                        SourceSpan = schemaEntryMarker.SourceSpan
                    }
                );
            }
        }
        
        [Theory]
        [ClassData(typeof(Parse_ValidSchemaEntry_Works_Data))]
        public void Parse_ValidSchemaEntry_Works(string value, SchemaEntry expected) {
            var result = ZuhParser.SchemaEntry.Parse(value);

            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }
    }
}