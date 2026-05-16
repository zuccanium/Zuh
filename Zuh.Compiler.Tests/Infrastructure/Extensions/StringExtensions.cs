using Zuh.Compiler.Ast;

namespace Zuh.Compiler.Tests.Infrastructure.Extensions {
    public static class StringExtensions {
        extension(string @this) {
            internal SourceSpan GetSpan(int n, string content) {
                var startIndex = 0;
                
                for(var i = 0; i <= n; i++) {
                    var index = @this.IndexOf(content, startIndex, StringComparison.InvariantCulture);

                    if(index == -1)
                        throw new InvalidOperationException("couldnt find span in string!");
                    
                    startIndex = index;
                }

                return new SourceSpan() {
                    Start = startIndex,
                    End = content.Length + startIndex
                };
            }
            
            internal SourceSpan GetSpan(string content)
                => @this.GetSpan(0, content);

            internal SourceSpan GetSpan()
                => new() {
                    Start = 0,
                    End = @this.Length
                };
        }
    }
}