namespace Zuh.Compiler.Diagnostics {
    public abstract record Warning : Diagnostic {
        public override DiagnosticSeverity Severity
            => DiagnosticSeverity.Warning;
    }
}