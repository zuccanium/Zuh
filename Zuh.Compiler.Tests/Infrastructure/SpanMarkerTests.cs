using Zuh.Compiler.Ast;
using static Zuh.Compiler.Tests.Infrastructure.SpanMarker;

namespace Zuh.Compiler.Tests.Infrastructure {
    public class SpanMarkerTests {
        [Fact]
        public void SpanMarker_BasicThing_Works() {
            var outerExpected = new SpanMarker() {
                Value = "yo hi",
                SourceSpan = new SourceSpan() {
                    Start = 0,
                    End = 5
                }
            };

            var innerExpected = new SpanMarker() {
                Value = "hi",
                SourceSpan = new SourceSpan() {
                    Start = 3,
                    End = 5
                }
            };
            
            Resolve(Mark(out var outer, $"yo {Mark(out var inner, "hi")}"));
            
            Assert.Equivalent(outerExpected, outer);
            Assert.Equivalent(innerExpected, inner);
        }
    }
}