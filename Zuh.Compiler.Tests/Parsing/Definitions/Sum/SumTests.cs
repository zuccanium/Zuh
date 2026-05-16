using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using Zuh.Compiler.Tests.Infrastructure.Extensions;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing.Definitions.Sum {
    public class SumTests {
        public class Parse_ValidSum_Works_Data : TheoryData<string, Ast.Sum> {
            public Parse_ValidSum_Works_Data() {
                for(var i = 0; i < 4; i++)
                    add(i);
            }

            private void add(int entryCount) {
                var entryNodes = Enumerable.Range(0, entryCount)
                    .SelectWithOut(
                        out var entryGetters,
                        (int n, out Func<SumEntry> outValue)
                            => CreateSumEntry(out outValue)
                    );
                
                Resolve(Mark(out var schemaMarker, $"[ {entryNodes.MarkAsJoined(", ")} ]"));
                
                Add(
                    schemaMarker.Value,
                    new Ast.Sum() {
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
        [ClassData(typeof(Parse_ValidSum_Works_Data))]
        public void Parse_ValidSum_Works(string value, Ast.Sum expected) {
            var result = ZuhParser.Sum.Parse(value);
            
            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }
    }
}