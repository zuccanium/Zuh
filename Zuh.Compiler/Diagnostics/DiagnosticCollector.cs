using System.Collections;

namespace Zuh.Compiler.Diagnostics {
    public class DiagnosticCollector : List<Diagnostic> {
        /// <summary>
        /// aggregates a bunch of <see cref="DiagnosticCollector"/>s into a single one.
        /// </summary>
        /// <param name="collectors">a bunch of <see cref="DiagnosticCollector"/>s</param>
        /// <returns>the aggregated <see cref="DiagnosticCollector"/></returns>
        public static DiagnosticCollector Merge(params IEnumerable<DiagnosticCollector> collectors) {
            var bigCollector = new DiagnosticCollector();

            foreach(var collector in collectors)
                bigCollector.AddRange(collector);
            
            return bigCollector;;
        }
    }
}