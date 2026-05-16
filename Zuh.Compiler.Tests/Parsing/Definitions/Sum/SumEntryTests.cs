using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using Zuh.Compiler.Tests.Infrastructure.Extensions;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing.Definitions.Sum {
    public class SumEntryTests {
        public class Parse_ValidSumEntry_Works_Data : TheoryData<string, SumEntry> {
            public Parse_ValidSumEntry_Works_Data() {
                for(var i = 0; i < 5; i++)
                    add(i);
            }

            private void add(int documentationCount) {
                var documentationLineNodes = Enumerable.Range(0, documentationCount)
                    .SelectWithOut(
                        out var documentationLineGetters,
                        (int _, out Func<DocumentationLine> outValue)
                            => CreateDocumentationLine(out outValue)
                    );

                var keyNode = CreateKey(out var keyGetter);

                var node = Mark(
                    out var sumEntryMarker,
                    $"{documentationLineNodes.MarkAsJoined("\n", true)}{keyNode}");
                
                Resolve(node);
                
                Add(
                    sumEntryMarker.Value,
                    new SumEntry() {
                        Key = keyGetter(),
                        DocumentationLines = [
                            ..documentationLineGetters
                                .Select(getter => getter())
                        ],
                        SourceSpan = sumEntryMarker.SourceSpan
                    }
                );
            }
        }
        
        [Theory]
        [ClassData(typeof(Parse_ValidSumEntry_Works_Data))]
        public void Parse_ValidSumEntry_Works(string value, SumEntry expected) {
            var result = ZuhParser.SumEntry.Parse(value);

            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }
    }
}