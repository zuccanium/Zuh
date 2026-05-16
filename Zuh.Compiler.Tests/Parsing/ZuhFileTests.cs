using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using Zuh.Compiler.Tests.Infrastructure.Extensions;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;
using static Zuh.Compiler.Tests.Infrastructure.SyntaxFactory;

namespace Zuh.Compiler.Tests.Parsing {
    public class ZuhFileTests {
        public class Parse_ValidZuhFile_Works_Data : TheoryData<string, ZuhFile> {
            public Parse_ValidZuhFile_Works_Data() {
                for(var i = 0; i < 4; i++)
                    add(i);
            }

            private void add(int statementCount) {
                var statementNodes = Enumerable.Range(0, statementCount)
                    .SelectWithOut(
                        out var statementGetters,
                        (int n, out Func<Statement> outValue)
                            => CreateStatement(out outValue)
                    );

                Resolve(Mark(out var zuhFileMarker, $"{statementNodes.MarkAsJoined("\n")}"));
                
                Add(
                    zuhFileMarker.Value,
                    new ZuhFile() {
                        RootStatements = [
                            ..statementGetters
                                .Select(getter => getter())
                        ],
                        SourceSpan = zuhFileMarker.SourceSpan
                    }
                );
            }
        }
        
        [Theory]
        [ClassData(typeof(Parse_ValidZuhFile_Works_Data))]
        public void Parse_ValidZuhFile_Works(string value, ZuhFile expected) {
            var result = ZuhParser.ZuhFile.Parse(value);
            
            Assert.True(result.Success);
            Assert.Equivalent(expected, result.Value);
        }
    }
}