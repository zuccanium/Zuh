using Zuh.Compiler.Ast;
using Zuh.Compiler.Diagnostics;
using Zuh.Compiler.Parsing;

namespace Zuh.Compiler.Semantics {
    /// <summary>
    /// encapsulates data about the resolution of an import and handles fetching the content.
    /// </summary>
    public interface IImportResolution {
        /// <summary>
        /// if the import was successfully resolved and the content can be confidently fetched.
        /// </summary>
        public bool Success { get; init; }
        
        /// <summary>
        /// unique unit id for the import.
        /// this is used by <see cref="Analyzers.CompilationAnalyzer"/> to cache compilation units.
        /// </summary>
        /// <remarks>
        /// it should be null if <see cref="Success"/> isnt true.
        /// </remarks>
        public string? Id { get; init; }

        /// <summary>
        /// converts the output of <see cref="FetchContent"/> into a parsed <see cref="ZuhFile"/>.
        /// </summary>
        /// <param name="diagnostics">the diagnostics from the parsing process</param>
        /// <returns>the parsed <see cref="ZuhFile"/></returns>
        public ZuhFile FetchFile(out DiagnosticCollector diagnostics);
    }
}