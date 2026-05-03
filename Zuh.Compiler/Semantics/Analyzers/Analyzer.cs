using Zuh.Compiler.Ast;
using Zuh.Compiler.Diagnostics;

namespace Zuh.Compiler.Semantics.Analyzers {
    public abstract class Analyzer {
        public DiagnosticCollector Diagnostics { get; private init; } = [];

        public abstract void Analyze();
    }
}