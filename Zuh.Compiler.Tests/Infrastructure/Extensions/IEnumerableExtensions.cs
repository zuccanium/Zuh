namespace Zuh.Compiler.Tests.Infrastructure.Extensions {
    public static class IEnumerableExtensions {
        public record struct MarkedAsJoined<T>(IEnumerable<T> Enumerable, string Joiner);

        public delegate TReturn SelectOutDelegate<TSource, TOut, TReturn>(TSource source, out TOut outValue);
        
        extension<T>(IEnumerable<T> @this) {
            public MarkedAsJoined<T> MarkAsJoined(string str)
                => new(@this, str);

            /// <summary>
            /// acts like Select but allows you to accumulate out parameters as well.
            /// </summary>
            /// <param name="outEnumerable">the accumulated out values.</param>
            /// <param name="selector">the selector that will be applied to each element.</param>
            /// <typeparam name="TOut">the type of the out parameter.</typeparam>
            /// <typeparam name="TReturn">the return value.</typeparam>
            /// <returns>the accumulated return values.</returns>
            public IEnumerable<TReturn> SelectWithOut<TOut, TReturn>(
                out IEnumerable<TOut> outEnumerable,
                SelectOutDelegate<T, TOut, TReturn> selector
            ) {
                var outList = new List<TOut>();
                var returnList = new List<TReturn>();
                
                foreach(var entry in @this) {
                    var returnValue = selector(entry, out var outValue);
                    
                    returnList.Add(returnValue);
                    outList.Add(outValue);
                }
                
                outEnumerable = outList;
                
                return returnList;
            }
        }
    }
}