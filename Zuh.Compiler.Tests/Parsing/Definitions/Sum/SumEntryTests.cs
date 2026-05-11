using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing.Definitions.Sum {
    public class SumEntryTests {
        [Fact]
        public void Parse_ValidSumEntry_Works() {
            Resolve(Mark(out var sumEntryMarker, $"{CreateKey(out var keyGetter)}"));
            
            var result = ZuhParser.SchemaEntry.Parse(sumEntryMarker.Value);

            var expected = new SumEntry() {
                Key = keyGetter(),
                DocumentationLines = [],
                SourceSpan = sumEntryMarker.SourceSpan
            };
            
            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }
    }
}