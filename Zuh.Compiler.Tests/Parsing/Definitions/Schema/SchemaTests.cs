using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using Zuh.Compiler.Tests.Infrastructure.Extensions;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing.Definitions.Schema {
    public class SchemaTests {
        public class Parse_ValidSchema_Works_Data : TheoryData<string, Ast.Schema> {
            public Parse_ValidSchema_Works_Data() {
                for(var i = 0; i < 4; i++)
                    add(i);
            }

            private void add(int entryCount) {
                var entryNodes = Enumerable.Range(0, entryCount)
                    .SelectWithOut(
                        out var entryGetters,
                        (int n, out Func<SchemaEntry> outValue)
                            => CreateSchemaEntry(out outValue)
                    );
                
                Resolve(Mark(out var schemaMarker, $"{{ {entryNodes.MarkAsJoined(", ")} }}"));
                
                Add(
                    schemaMarker.Value,
                    new Ast.Schema() {
                        Entries = [
                            ..entryGetters
                                .Select(getter => getter())
                        ],
                        SourceSpan = schemaMarker.SourceSpan
                    }
                );
            }
        }
        
        [Theory]
        [ClassData(typeof(Parse_ValidSchema_Works_Data))]
        public void Parse_ValidSchema_Works(string value, Ast.Schema expected) {
            var result = ZuhParser.Schema.Parse(value);
            
            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }
    }
}