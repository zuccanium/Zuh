using Zuh.Compiler.Ast;
using Zuh.Compiler.Semantics.Analyzers;

namespace Zuh.Compiler.Generation {
    public class Generator {
        public required CompilationAnalyzer Analyzer { get; init; }
    }
}