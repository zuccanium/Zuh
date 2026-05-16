using Pidgin;
using Zuh.Compiler.Ast;
using Zuh.Compiler.Parsing;
using Zuh.Compiler.Tests.Infrastructure.Extensions;

namespace Zuh.Compiler.Tests.Parsing.Atoms.Literals {
    public class StringLiteralTests {
        public class Parse_StringLiteral_Works_Data : TheoryData<string, StringLiteral> {
            public Parse_StringLiteral_Works_Data() {
                addStringTest("something");
                addStringTest("it works with spaces");
                addStringTest("it works with 'single quotes'");
                addStringTest(" ");
                
                // special case doesnt conform
                Add(
                    "\"\"",
                    new StringLiteral() {
                        Value = "",
                        SourceSpan = new SourceSpan() {
                            Start = 0,
                            End = 2
                        }
                    }
                );
            }

            private void addStringTest(string content) {
                var value = $"\"{content}\"";
                
                Add(
                    value,
                    new StringLiteral() {
                        Value = content,
                        SourceSpan = value.GetSpan()
                    }
                );
            }
        }
        
        [Theory]
        [ClassData(typeof(Parse_StringLiteral_Works_Data))]
        public void Parse_StringLiteral_Works(string value, StringLiteral expected) {
            var result = ZuhParser.StringLiteral.Parse(value);
            
            Assert.True(result.Success);
            Assert.Equal(expected, result.Value);
        }
    }
}